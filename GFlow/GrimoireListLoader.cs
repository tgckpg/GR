using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

using GFlow.Controls;
using GFlow.Models.Interfaces;
using GFlow.Models.Procedure;

namespace GR.GFlow
{
	using Model.Book.Spider;

	class GrimoireListLoader : Procedure, IProcessList
	{
		public static readonly string ID = typeof( GrimoireListLoader ).Name;

		internal static Type GRPropertyPage;
		public override Type PropertyPage => GRPropertyPage;

		public string ItemPattern { get; set; }
		public string ItemParam { get; set; }

		public bool HasSubProcs { get { return ItemProcs.HasProcedures; } }
		public bool HasBookSpider { get { return BookSpider.HasProcedures; } }

		public Uri BannerSrc { get; set; }
		public string BannerPath
		{
			get { return BannerSrc?.ToString(); }
			set
			{
				try
				{
					BannerSrc = new Uri( value );
					NotifyChanged( "BannerSrc" );
				}
				catch ( Exception ) { }
			}
		}

		public string ZoneName { get; set; }

		private ProcManager ItemProcs
		{
			get => ProcessNodes[ 0 ].SubProcedures;
			set => ProcessNodes[ 0 ].SubProcedures = value;
		}

		private ProcManager BookSpider
		{
			get => ProcessNodes[ 1 ].SubProcedures;
			set => ProcessNodes[ 1 ].SubProcedures = value;
		}

		public IList<IProcessNode> ProcessNodes { get; private set; }

		public GrimoireListLoader()
			: base( ProcType.LIST )
		{
			StringResources stx = StringResources.Load( "AppResources", "/GFlow/Resources" );
			ProcessNodes = new LLNode[]
			{
				new LLNode() { Key = stx.Text( "SubProcs", "/GFlow/Resources" ) }
				, new LLNode() { Key = stx.Text( "BookSpider" ) }
			};
		}

		public void SetProp( string PropName, string Val )
		{
			switch ( PropName )
			{
				case "ItemPattern": ItemPattern = Val; break;
				case "ItemParam": ItemParam = Val; break;
			}
		}

		public override void ReadParam( XParameter Param )
		{
			base.ReadParam( Param );

			ItemPattern = Param.GetValue( "ItemPattern" );
			ItemParam = Param.GetValue( "ItemParam" );
			BannerPath = Param.GetValue( "Banner" );
			ZoneName = Param.GetValue( "ZoneName" );

			ItemProcs = new ProcManager( Param.Parameter( "ItemProcs" ) );
			BookSpider = new ProcManager( Param.Parameter( "BookSpider" ) );
		}

		public override XParameter ToXParam()
		{
			XParameter Param = base.ToXParam();
			Param.SetValue( new XKey[]
			{
				new XKey( "ItemPattern", ItemPattern )
				, new XKey( "ItemParam", ItemParam )
				, new XKey( "Banner", BannerPath )
				, new XKey( "ZoneName", ZoneName )
			} );

			XParameter EProc = ItemProcs.ToXParam();
			EProc.Id = "ItemProcs";
			Param.SetParameter( EProc );

			XParameter SProc = BookSpider.ToXParam();
			SProc.Id = "BookSpider";
			Param.SetParameter( SProc );

			return Param;
		}

		public void ImportSpider( XParameter SpiderDef )
		{
			ProcManager PM = new ProcManager( SpiderDef );
			BookSpider = PM;
			NotifyChanged( "HasBookSpider" );
		}

		public override async Task<ProcConvoy> Run( ICrawler Crawler, ProcConvoy Convoy )
		{
			Convoy = await base.Run( Crawler, Convoy );

			if ( !TryGetConvoy(
				out ProcConvoy UsableConvoy
				, ( P, C ) =>
					C.Payload is IEnumerable<IStorageFile>
					|| C.Payload is IEnumerable<string>
					|| C.Payload is IStorageFile
					|| C.Payload is string
				) ) return Convoy;

			List<BookInstruction> SpItemList = null;

			// Search for the closest Instruction Set
			ProcConvoy SpiderInst = ProcManager.TracePackage(
				Convoy,
				( P, C ) => C.Payload is IEnumerable<BookInstruction>
			);

			if ( SpiderInst != null )
			{
				SpItemList = ( List<BookInstruction> ) SpiderInst.Payload;
			}

			if ( SpItemList == null )
			{
				SpItemList = new List<BookInstruction>();
			}

			ProcPassThru PPass = new ProcPassThru( new ProcConvoy( this, SpItemList ) );
			ProcConvoy KnownBook = ProcManager.TracePackage( Convoy, ( P, C ) => C.Payload is BookInstruction );

			if ( UsableConvoy.Payload is IEnumerable<IStorageFile> ISFs )
			{
				foreach ( IStorageFile ISF in ISFs )
				{
					string Content = await ISF.ReadString();
					await SearchBooks( Crawler, SpItemList, PPass, KnownBook, Content );
				}
			}
			else if ( UsableConvoy.Payload is IEnumerable<string> Contents )
			{
				foreach ( string Content in Contents )
				{
					await SearchBooks( Crawler, SpItemList, PPass, KnownBook, Content );
				}
			}
			else if ( UsableConvoy.Payload is IStorageFile ISF )
			{
				string Content = await ISF.ReadString();
				await SearchBooks( Crawler, SpItemList, PPass, KnownBook, Content );
			}
			else // string
			{
				await SearchBooks( Crawler, SpItemList, PPass, KnownBook, ( string ) UsableConvoy.Payload );
			}

			return new ProcConvoy( this, SpItemList );
		}

		private async Task SearchBooks( ICrawler Crawler, List<BookInstruction> ItemList, ProcPassThru PPass, ProcConvoy KnownBook, string Content )
		{
			ProcFind.RegItem RegParam = new ProcFind.RegItem( ItemPattern, ItemParam, true );

			if ( !RegParam.Validate() ) return;

			MatchCollection matches = RegParam.RegExObj.Matches( Content );
			StringResources ste = StringResources.Load( "Error" );

			foreach ( Match match in matches )
			{
				if ( HasSubProcs && RegParam.Valid )
				{
					string FParam = string.Format(
						RegParam.Format
						, match.Groups
							.Cast<Group>()
							.Select( g => g.Value )
							.ToArray()
					);

					ProcConvoy ItemConvoy = await ItemProcs.CreateSpider( Crawler ).Crawl( new ProcConvoy( PPass, FParam ) );

					string Id = await GetId( ItemConvoy );
					if ( string.IsNullOrEmpty( Id ) )
					{
						Crawler.PLog( this, ste.Str( "NoIdForBook" ), LogType.WARNING );
						continue;
					}

					ItemConvoy = ProcManager.TracePackage( ItemConvoy, ( P, C ) => C.Payload is BookInstruction );

					if ( !( ItemConvoy == null || ItemConvoy == KnownBook ) )
					{
						BookInstruction BInst = ( BookInstruction ) ItemConvoy.Payload;
						ItemList.Add( BInst );

						if ( HasBookSpider ) BInst.PlaceDefs( Id, BookSpider );
					}
					else
					{
						Crawler.PLog( this, ste.Str( "NotABook" ), LogType.WARNING );
					}
				}
			}
		}

		private async Task<string> GetId( ProcConvoy Convoy )
		{
			Convoy = ProcManager.TracePackage(
				Convoy, ( P, C ) =>
					C.Payload is IEnumerable<IStorageFile>
					|| C.Payload is IEnumerable<string>
					|| C.Payload is IStorageFile
					|| C.Payload is string
			);

			if ( Convoy == null ) return null;

			switch ( Convoy.Payload )
			{
				case IEnumerable<IStorageFile> ISFs:
					return await ISFs.FirstOrDefault()?.ReadString();
				case IEnumerable<string> Contents:
					return Contents.FirstOrDefault();
				case IStorageFile ISF:
					return await ISF.ReadString();
			}

			return ( string ) Convoy.Payload;
		}

		private class LLNode : IProcessNode
		{
			public string Key { get; set; }
			public ProcManager SubProcedures { get; set; } = new ProcManager();
		}

	}
}