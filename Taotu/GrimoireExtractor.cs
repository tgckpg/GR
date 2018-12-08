using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

using GFlow.Controls;
using GFlow.Crawler;
using GFlow.Models.Interfaces;
using GFlow.Models.Procedure;

namespace GR.GFlow
{
	using Model.Book;
	using Model.Book.Spider;
	using Settings;

	class GrimoireExtractor : Procedure, IProcessList 
	{
		public string TargetUrl { get; internal set; }
		public bool Incoming { get; internal set; }

		private static IEnumerable<GenericData<PropType>> _PossibleTypes;
		public static IEnumerable<GenericData<PropType>> PossibleTypes => _PossibleTypes ?? ( _PossibleTypes = GenericData<PropType>.Convert( Enum.GetValues( typeof( PropType ) ) ) );

		internal static Type GRPropertyPage;
		public override Type PropertyPage => GRPropertyPage;

		public IList<IProcessNode> ProcessNodes { get; set; }

		public GrimoireExtractor()
			: base( ProcType.EXTRACT )
		{
			ProcessNodes = new ObservableCollection<IProcessNode>();
		}

		public override async Task<ProcConvoy> Run( ICrawler Crawler, ProcConvoy Convoy )
		{
			await base.Run( Crawler, Convoy );

			string LoadUrl = TargetUrl;
			string Content = "";

			ProcConvoy UsableConvoy = ProcManager.TracePackage(
				Convoy, ( P, C ) =>
					C.Payload is IEnumerable<IStorageFile>
					|| C.Payload is IEnumerable<string>
					|| C.Payload is IStorageFile
					|| C.Payload is string
			);

			IStorageFile ISF = null;

			if ( UsableConvoy != null )
			{
				Crawler.PLog( this, Res.RSTR( "IncomingCheck" ), LogType.INFO );

				if ( UsableConvoy.Payload is IEnumerable<IStorageFile> )
				{
					ISF = ( UsableConvoy.Payload as IEnumerable<IStorageFile> ).FirstOrDefault();
				}
				else if ( UsableConvoy.Payload is IStorageFile )
				{
					ISF = ( IStorageFile ) UsableConvoy.Payload;
				}

				if ( Incoming )
				{
					if ( UsableConvoy.Payload is IEnumerable<string> )
					{
						LoadUrl = ( UsableConvoy.Payload as IEnumerable<string> ).FirstOrDefault();
					}
					else if ( UsableConvoy.Payload is string )
					{
						LoadUrl = ( string ) UsableConvoy.Payload;
					}

					if ( ISF == null && string.IsNullOrEmpty( LoadUrl ) )
					{
						Crawler.PLog( this, Res.RSTR( "NoUsablePayload" ), LogType.WARNING );
						return Convoy;
					}

					if ( !string.IsNullOrEmpty( LoadUrl ) )
					{
						LoadUrl = WebUtility.HtmlDecode( LoadUrl );
					}
				}
				else // Incomings are Content
				{
					if ( UsableConvoy.Payload is IEnumerable<string> )
					{
						Content = string.Join( "\n", ( IEnumerable<string> ) UsableConvoy.Payload );
					}
					else if ( UsableConvoy.Payload is string )
					{
						Content = ( string ) UsableConvoy.Payload;
					}
				}
			}

			ProcConvoy BookConvoy = ProcManager.TracePackage( Convoy, ( D, C ) => C.Payload is BookItem );

			BookItem BookInst = ( BookConvoy == null )
				? new BookInstruction()
				: ( BookConvoy.Payload as BookItem )
				;

			if ( !string.IsNullOrEmpty( LoadUrl ) )
			{
				BookInst.ReadParam( AppKeys.BINF_ORGURL, LoadUrl );

				if ( string.IsNullOrEmpty( Content ) && ISF == null )
				{
					ISF = await Crawler.DownloadSource( LoadUrl );
				}
			}

			if ( ISF != null ) Content = await ISF.ReadString();

			// Event Content is null, Props might be still extractable as there might be some predefined props exists
			await ExtractProps( Crawler, BookInst, Content );

			return new ProcConvoy( this, BookInst );
		}

		private async Task ExtractProps( ICrawler Crawler, BookItem Inst, string Content )
		{
			foreach( PropExt Extr in ProcessNodes )
			{
				if ( !Extr.Enabled ) continue;

				string PropValue = MatchSingle( Extr, Content );

				if ( Extr.SubProcedures.HasProcedures )
				{
					Crawler.PLog( this, Res.RSTR( "SubProcRun" ), LogType.INFO );
					ProcPassThru PPass = new ProcPassThru( new ProcConvoy( this, Inst ) );
					ProcConvoy SubConvoy = await Extr.SubProcedures.CreateSpider( Crawler ).Crawl( new ProcConvoy( PPass, PropValue ) );

					// Process ReceivedConvoy
					if ( SubConvoy.Payload is string )
						PropValue = ( string ) SubConvoy.Payload;
					else if ( SubConvoy.Payload is IEnumerable<string> )
						PropValue = string.Join( "\n", ( IEnumerable<string> ) SubConvoy.Payload );
					else if ( SubConvoy.Payload is IStorageFile )
						PropValue = await ( ( IStorageFile ) SubConvoy.Payload ).ReadString();
					else if ( SubConvoy.Payload is IEnumerable<IStorageFile> )
						PropValue = await ( ( IEnumerable<IStorageFile> ) SubConvoy.Payload ).First().ReadString();
					else continue;
				}

				// If the website split a single property into serveral pages
				// That website is stupid. Would not support.
				if ( !Inst.ReadParam( Extr.PType.ToString(), PropValue ) )
				{
					Crawler.PLog( this, Res.RSTR( "InvalidParam", Extr.PType ), LogType.WARNING );
				}
			}
		}

		// Match a single item
		public string MatchSingle( PropExt R, string Content )
		{
			// Set the value if patter left empty
			if ( string.IsNullOrEmpty( R.Pattern )
				&& !string.IsNullOrEmpty( R.Format )
			) return R.Format;

			MatchCollection matches = R.RegExObj.Matches( Content );

			string PropValue = "";
			foreach ( Match match in matches )
			{
				PropValue += string.Format(
					R.Format.Unescape()
					, match.Groups
						.Cast<Group>()
						.Select( g => g.Value )
						.ToArray()
				);
			}

			return PropValue;
		}

		public override void ReadParam( XParameter Param )
		{
			base.ReadParam( Param );

			TargetUrl = Param.GetValue( "TargetUrl" );
			Incoming = Param.GetBool( "Incoming" );

			XParameter[] ExtParams = Param.Parameters( "i" );
			foreach ( XParameter ExtParam in ExtParams )
			{
				ProcessNodes.Add( new PropExt( ExtParam ) );
			}
		}

		public override XParameter ToXParam()
		{
			XParameter Param = base.ToXParam();

			Param.SetValue( new XKey[] {
				new XKey( "TargetUrl", TargetUrl )
				, new XKey( "Incoming", Incoming )
			} );

			int i = 0;
			foreach( PropExt Extr in ProcessNodes )
			{
				XParameter ExtParam = Extr.ToXParam();
				ExtParam.Id += i;
				ExtParam.SetValue( new XKey( "i", i++ ) );

				Param.SetParameter( ExtParam );
			}

			return Param;
		}

		public class PropExt : ProcFind.RegItem, IProcessNode, INamable
		{
			public static readonly Type BINF = typeof( PropType );

			public string Name { get => Key; set => throw new InvalidOperationException(); }

			public string Key => BookItem.PropertyName( PType );
			public ProcManager SubProcedures { get; set; } = new ProcManager();

			public bool HasSubProcs => SubProcedures.HasProcedures;

			public override bool Enabled
			{
				get
				{
					if ( Valid
						&& string.IsNullOrEmpty( Pattern )
						&& !string.IsNullOrEmpty( Format ) )
						return true;

					return base.Enabled;
				}

				set { base.Enabled = value; }
			}

			public IEnumerable<GenericData<PropType>> Types => PossibleTypes;
			public GenericData<PropType> SelectedType => Types.First( x => x.Data == PType );
			public PropType PType { get; set; }

			public PropExt( PropType PType = PropType.Others )
			{
				this.PType = PType;
				Enabled = true;
			}

			public PropExt( XParameter Param )
				: base( Param )
			{
				string SType = Param.GetValue( "Type" );

				XParameter Sub = Param.Parameter( "SubProc" );
				if ( Sub != null ) SubProcedures.ReadParam( Sub );

				PType = Enum.GetValues( BINF )
					.Cast<PropType>()
					.FirstOrDefault( x => Enum.GetName( BINF, x ) == SType );
			}

			public override XParameter ToXParam()
			{
				XParameter Param =  base.ToXParam();
				Param.SetValue( new XKey( "Type", PType ) );

				XParameter SubParam = SubProcedures.ToXParam();
				SubParam.Id = "SubProc";
				Param.SetParameter( SubParam );

				return Param;
			}
		}
	}
}