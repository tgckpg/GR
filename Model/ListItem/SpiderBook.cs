using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

using libtaotu.Pages;
using libtaotu.Controls;
using libtaotu.Crawler;
using libtaotu.Models.Procedure;

namespace GR.Model.ListItem
{
	using Book.Spider;
	using Interfaces;
	using Resources;
	using Settings;

	sealed class SpiderBook : LocalBook, IMetaSpider
	{
		public override string Name
		{
			get
			{
				if ( !Processed && CanProcess && string.IsNullOrEmpty( base.Name ) )
					base.Name = new StringResources().Text( "BookSpider" );

				return base.Name;
			}

			set { base.Name = value; }
		}

		public override string Desc
		{
			get
			{
				if ( !Processed && CanProcess && string.IsNullOrEmpty( base.Desc ) )
					base.Desc = new StringResources().Text( "PleaseClick" );

				return base.Desc;
			}

			set { base.Desc = value; }
		}

		// Determine if instructions has Chakra process inside
		// Which needs WebView, which needs UI Thread
		public bool HasChakra { get; private set; }

		private ProcManager ProcMan;
		private BookInstruction BInst;
		public XRegistry PSettings { get; private set; }

		public string MetaRoot => FileLinks.ROOT_SPIDER_VOL + ZoneId + "/";
		public string MetaLocation => MetaRoot + ZItemId + ".xml";

		private SpiderBook() { }

		private void InitProcMan()
		{
			if ( ProcMan != null ) return;
			ProcMan = new ProcManager();
			XParameter Param = PSettings.Parameter( "Procedures" );

			if ( Param == null )
			{
				Processed = true;
				CanProcess = false;
				ProcessSuccess = false;
				return;
			}

			ProcMan.ReadParam( Param );
		}

		public static async Task<SpiderBook> ImportFile( string ProcSetting, bool Save )
		{
			SpiderBook Book = new SpiderBook();
			Book.PSettings = new XRegistry( ProcSetting, null );

			Book.InitProcMan();
			Book.ZoneId = "[Local]";
			Book.ZItemId = Book.ProcMan.GUID;

			await Book.TestProcessed();

			if ( Book.CanProcess || Book.ProcessSuccess )
			{
				Book.PSettings.Location = Book.MetaLocation;

				if ( Save )
				{
					Book.PSettings.Save();
				}
			}

			return Book;
		}

		public static XRegistry GetSettings( string ZoneId, string ZItemId )
		{
			SpiderBook Book = new SpiderBook { ZoneId = ZoneId, ZItemId = ZItemId };
			return new XRegistry( "<ProcSpider />", Book.MetaLocation );
		}

		public static async Task<SpiderBook> CreateSAsync( string ZoneId, string ZItemId, XParameter SpiderDef )
		{
			SpiderBook Book = new SpiderBook
			{
				ZoneId = ZoneId,
				ZItemId = ZItemId
			};

			if ( SpiderDef == null )
			{
				Book.PSettings = new XRegistry( "<ProcSpider />", Book.MetaLocation );
			}
			else
			{
				Book.PSettings = new XRegistry( "<ProcSpider />", Book.MetaLocation, false );
				Book.PSettings.SetParameter( SpiderDef );
			}

			await Book.TestProcessed();
			return Book;
		}

		public static Task<SpiderBook> CreateSAsync( string ZItemId ) => CreateSAsync( null, ZItemId, null );

		public override async Task Reload()
		{
			ProcMan = null;
			PSettings = new XRegistry( "<ProcSpider />", MetaLocation );
			ProcParameter.DestroyParams( PSettings );

			BInst?.ClearCover();

			await TestProcessed();
		}

		protected override async Task Run()
		{
			InitProcMan();
			ProceduralSpider Spider = ProcMan.CreateSpider();

			string Payload = PSettings.Parameter( "METADATA" )?.GetValue( "payload" );
			ProcConvoy Convoy = ProcParameter.RestoreParams( PSettings, string.IsNullOrEmpty( Payload ) ? null : Payload );

			if ( Convoy.Dispatcher is ProcParameter )
			{
				Convoy = new ProcConvoy( new ProcPassThru( Convoy, ProcType.FEED_RUN ), null );
			}

			BInst = new BookInstruction( ZoneId, ZItemId );

			ProcPassThru PThru = new ProcPassThru( Convoy );

			// This values is set from PlaceDef in BookInstruction by ZoneSpider
			if ( BInst.Entry.Meta.ContainsKey( AppKeys.GLOBAL_SSID ) )
			{
				// Wrap another level
				PThru = new ProcPassThru( new ProcConvoy( PThru, BInst.Entry.Meta[ AppKeys.GLOBAL_SSID ] ) );
			}

			Convoy = await Spider.Crawl( new ProcConvoy( PThru, BInst ) );

			HasChakra = ProcManager.TracePackage( Convoy, ( D, C ) => D is ProcChakra ) != null;

			ProcParameter.StoreParams( Convoy, PSettings );
			Convoy = ProcManager.TracePackage( Convoy, ( D, C ) => C.Payload is BookInstruction );

			if ( Convoy == null ) throw new Exception( "Unable to find Book Info" );

			BInst = ( BookInstruction ) Convoy.Payload;

			Name = BInst.Title;
			Desc = BInst.LastUpdateDate;

			XParameter XParam = new XParameter( "ProcessState" );
			XParam.SetValue( new XKey[] {
				new XKey( "Success", true )
				, new XKey( "HasChakra", HasChakra )
			} );

			PSettings.SetParameter( XParam );
			PSettings.Save();

			BInst.ZoneId = ZoneId;
			BInst.ZItemId = ZItemId;
			BInst.SaveInfo();
		}

		protected override async Task TestProcessed()
		{
			await Task.Run( () =>
			{
				try
				{
					InitProcMan();

					if( ProcMan.ProcList.Count == 0 )
					{
						throw new ArgumentNullException( "ProcList is empty" );
					}

					XParameter SParam = PSettings.Parameter( "ProcessState" );

					if ( SParam != null
						&& ( ProcessSuccess = SParam.GetBool( "Success" ) ) )
					{
						BInst = new BookInstruction( ZoneId, ZItemId );
						Name = BInst.Title;
						Desc = BInst.LastUpdateDate;
					}

					CanProcess = true;
				}
				catch ( Exception ex )
				{
					Logger.Log( ID, ex.Message );
					Name = ex.Message;
					Desc = "ERROR";
					CanProcess = false;
					ProcessSuccess = false;
				}
			} );
		}

		public ProcConvoy GetPPConvoy()
		{
			return ProcParameter.RestoreParams( PSettings, null );
		}

		public void AssignId( string Id )
		{
			if ( ProcMan.GUID == Id ) return;

			ProcMan.GUID = Id;
			if ( ProcMan.GUID == Id )
			{
				string OldRoot = MetaRoot;

				string OId = ZItemId;
				ZItemId = Id;

				try
				{
					XParameter Param = PSettings.Parameter( "Procedures" );
					Param.SetValue( new XKey( "Guid", Id ) );
					PSettings.SetParameter( Param );

					// Begin Move location
					PSettings.Location = MetaLocation;

					Shared.Storage.MoveDir( OldRoot, MetaRoot );
					XParameter METADATA = PSettings.Parameter( "METADATA" );

					if ( METADATA != null )
					{
						string Sid = METADATA.GetValue( "sid" );
						if ( Sid != null )
						{
							METADATA.SetValue( new XKey( "payload", Sid ) );
							METADATA.RemoveKey( "sid" );

							PSettings.SetParameter( METADATA );
						}
					}

					BInst = new BookInstruction( ZoneId, ZItemId );

					PSettings.Save();
				}
				catch ( Exception )
				{
					BInst = null;
					Processed = false;
					Logger.Log( ID, string.Format( "Failed to move SVol: {0} => {1}", OldRoot, MetaRoot ), LogType.WARNING );
				}
			}
		}

		public async Task<SpiderBook> Clone()
		{
			XParameter Param = PSettings.Parameter( "Procedures" );
			Param.SetValue( new XKey( "Guid", Guid.NewGuid() ) );
			PSettings.SetParameter( Param );

			SpiderBook Book = await ImportFile( PSettings.ToString(), true );
			Book.Name += " - Copy";

			Param.SetValue( new XKey( "Guid", ZItemId ) );
			PSettings.SetParameter( Param );
			return Book;
		}

		public BookInstruction GetBook()
		{
			return BInst;
		}

		override protected void MessageBus_OnDelivery( Message Mesg )
		{
			if ( !( Mesg.Payload is ProceduresPanel.PanelLog ) ) return;

			// ProceduresPanel.PanelLog PLog = ( ProceduresPanel.PanelLog ) Mesg.Payload;
			Desc = Mesg.Content;
		}

		override public void RemoveSource()
		{
			if ( Shared.Storage.DirExist( FileLinks.ROOT_SPIDER_VOL + ZItemId ) )
			{
				Shared.Storage.RemoveDir( FileLinks.ROOT_SPIDER_VOL + ZItemId );
			}

			Processed = false;
			ProcessSuccess = false;
			CanProcess = false;

			Desc = "Script is unavailable";
		}
	}
}