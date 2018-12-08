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
	using Model.Interfaces;

	class GrimoireMarker : Procedure, IProcessList
	{
		public static readonly string ID = typeof( GrimoireMarker ).Name;

		public string VolPattern { get; set; }
		public string VolTitle { get; set; }
		public string VolParam { get; set; }
		public string EpPattern { get; set; }
		public string EpTitle { get; set; }
		public string EpParam { get; set; }

		public bool VolAsync
		{
			get { return VolProcs.Async; }
			set
			{
				VolProcs.Async = value;
				NotifyChanged( "VolAsync" );
			}
		}
		public bool EpAsync
		{
			get { return EpProcs.Async; }
			set
			{
				EpProcs.Async = value;
				NotifyChanged( "EpAsync" );
			}
		}

		public bool HasVolProcs { get { return VolProcs.HasProcedures; } }
		public bool HasEpProcs { get { return EpProcs.HasProcedures; } }

		private ProcManager VolProcs
		{
			get => ProcessNodes[ 0 ].SubProcedures;
			set => ProcessNodes[ 0 ].SubProcedures = value;
		}
		private ProcManager EpProcs
		{
			get => ProcessNodes[ 1 ].SubProcedures;
			set => ProcessNodes[ 1 ].SubProcedures = value;
		}

		internal static Type GRPropertyPage;
		public override Type PropertyPage => GRPropertyPage;

		public IList<IProcessNode> ProcessNodes { get; private set; }

		public GrimoireMarker()
			: base( ProcType.MARK )
		{
			StringResources stx = StringResources.Load( "Book" );
			ProcessNodes = new MarkerNode[]
			{
				new MarkerNode(){ Key = stx.Text( "VolumeMapping" ) }
				, new MarkerNode(){ Key = stx.Text( "ChapterMapping" ) }
			};
		}

		public void SetProp( string PropName, string Val )
		{
			switch( PropName )
			{
				case "VolTitle": VolTitle = Val; break;
				case "VolPattern": VolPattern = Val; break;
				case "VolParam": VolParam = Val; break;
				case "EpTitle": EpTitle = Val; break;
				case "EpPattern": EpPattern = Val; break;
				case "EpParam": EpParam = Val; break;
			}
		}

		public override void ReadParam( XParameter Param )
		{
			base.ReadParam( Param );

			VolTitle = Param.GetValue( "VolTitle" );
			VolPattern = Param.GetValue( "VolPattern" );
			VolParam = Param.GetValue( "VolParam" );
			EpTitle = Param.GetValue( "EpTitle" );
			EpPattern = Param.GetValue( "EpPattern" );
			EpParam = Param.GetValue( "EpParam" );

			VolProcs = new ProcManager( Param.Parameter( "VolProcs" ) );
			EpProcs = new ProcManager( Param.Parameter( "EpProcs" ) );
		}

		public override XParameter ToXParam()
		{
			XParameter Param = base.ToXParam();
			Param.SetValue( new XKey[]
			{
				new XKey( "VolTitle", VolTitle )
				, new XKey( "VolPattern", VolPattern )
				, new XKey( "VolParam", VolParam )
				, new XKey( "EpTitle", EpTitle )
				, new XKey( "EpPattern", EpPattern )
				, new XKey( "EpParam", EpParam )
			} );

			XParameter VProc = VolProcs.ToXParam();
			VProc.Id = "VolProcs";
			Param.SetParameter( VProc  );

			XParameter EProc = EpProcs.ToXParam();
			EProc.Id = "EpProcs";
			Param.SetParameter( EProc );

			return Param;
		}

		public override async Task<ProcConvoy> Run( ICrawler Crawler, ProcConvoy Convoy )
		{
			Convoy = await base.Run( Crawler, Convoy );

			ProcConvoy UsableConvoy;
			if ( !TryGetConvoy( out UsableConvoy, ( P, C ) =>
			{
				if ( P.Type.HasFlag( ProcType.FIND ) || P.Type.HasFlag( ProcType.URLLIST ) )
				{
					return C.Payload is IEnumerable<IStorageFile>;
				}
				return false;
			}
			) ) return Convoy;

			IInstructionSet SpTOC = null;

			// Search for the closest Instruction Set
			ProcConvoy SpiderInst = ProcManager.TracePackage(
				Convoy
				, ( P, C ) => {
					return C.Payload is IInstructionSet;
				}
			);

			if( SpiderInst != null )
			{
				SpTOC = SpiderInst.Payload as IInstructionSet;
			}

			if( SpTOC == null )
			{
				SpTOC = new BookInstruction();
			}

			IEnumerable<IStorageFile> ISFs = UsableConvoy.Payload as IEnumerable<IStorageFile>;

			bool VTitleAddOnce = false;
			ProcPassThru PPass = new ProcPassThru( new ProcConvoy( this, SpTOC ) );

			foreach( IStorageFile ISF in ISFs )
			{
				string Content = await ISF.ReadString();


				ProcFind.RegItem RegTitle = new ProcFind.RegItem( VolPattern, VolTitle, true );
				ProcFind.RegItem RegParam = new ProcFind.RegItem( VolPattern, VolParam, true );

				if ( !( RegTitle.Validate() || RegParam.Validate() ) ) continue;

				MatchCollection matches = RegTitle.RegExObj.Matches( Content );
				foreach ( Match match in matches )
				{
					if ( VTitleAddOnce ) break;
					VolInstruction VInst = null;
					if( RegTitle.Valid )
					{
						string FTitle = string.Format(
							RegTitle.Format
							, match.Groups
								.Cast<Group>()
								.Select( g => g.Value )
								.ToArray()
						);

						if( string.IsNullOrEmpty( RegTitle.Pattern ) )
						{
							VTitleAddOnce = true;
						}

						VInst = new VolInstruction(
							VTitleAddOnce ? SpTOC.LastIndex : match.Index
							, FTitle
						);

						VInst.ProcMan = VolProcs;
						SpTOC.PushInstruction( VInst );
					}
					else
					{
						Crawler.PLog( this, Res.RSTR( "InvalidPattern" ), LogType.WARNING );
						continue;
					}

					if ( HasVolProcs && RegParam.Valid )
					{
						string FParam = string.Format(
							RegParam.Format
							, match.Groups
								.Cast<Group>()
								.Select( g => g.Value )
								.ToArray()
						);

						if( VolAsync )
						{
							VInst.SetProcId( VolProcs.GUID );
							VInst.PushConvoyParam( FParam );
						}
						else
						{
							ProcConvoy VolConvoy = await VolProcs.CreateSpider( Crawler ).Crawl( new ProcConvoy( PPass, FParam ) );
						}
					}
				}

				RegTitle = new ProcFind.RegItem( EpPattern, EpTitle, true );
				RegParam = new ProcFind.RegItem( EpPattern, EpParam, true );

				if ( !( RegTitle.Validate() || RegParam.Validate() ) ) continue;

				matches = RegTitle.RegExObj.Matches( Content );
				foreach ( Match match in matches )
				{
					EpInstruction EInst = null;
					if( RegTitle.Valid )
					{
						string FTitle = string.Format(
							RegTitle.Format
							, match.Groups
								.Cast<Group>()
								.Select( g => g.Value )
								.ToArray()
						);

						EInst = new EpInstruction(
							VTitleAddOnce ? SpTOC.LastIndex : match.Index
							, FTitle
						);
						EInst.ProcMan = EpProcs;
						SpTOC.PushInstruction( EInst );
					}
					else
					{
						Crawler.PLog( this, Res.RSTR( "InvalidPattern" ), LogType.WARNING );
						continue;
					}

					if ( HasEpProcs && RegParam.Valid )
					{
						string FParam = string.Format(
							RegParam.Format
							, match.Groups
								.Cast<Group>()
								.Select( g => g.Value )
								.ToArray()
						);

						if( EpAsync )
						{
							EInst.SetProcId( EpProcs.GUID );
							EInst.PushConvoyParam( FParam );
						}
						else
						{
							ProcConvoy EpConvoy = await EpProcs.CreateSpider().Crawl( new ProcConvoy( PPass, FParam ) );
						}
					}
				}
			}

			return new ProcConvoy( this, SpTOC );
		}

		private class MarkerNode : IProcessNode
		{
			public string Key { get; set; }
			public ProcManager SubProcedures { get; set; } = new ProcManager();
		}

	}
}