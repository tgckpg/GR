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
	using Database.Models;
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
					base.Desc = new StringResources().Text( "UnprocessedItem" );

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

		public void MarkUnprocessed() { Processed = false; }

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
			Book.ZoneId = AppKeys.ZLOCAL;
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

		private void SLog( string Mesg, LogType LT )
		{
			Desc = Mesg;
		}

		private void SLog( Procedure P, string Mesg, LogType LT )
		{
			Desc = Mesg;
		}

		protected override async Task Run()
		{
			InitProcMan();
			ProceduralSpider Spider = ProcMan.CreateSpider();
			Spider.Log = SLog;
			Spider.PLog = SLog;

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

			if ( Spider.LastException is OperationCanceledException )
			{
				throw Spider.LastException;
			}

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

		protected override Task TestProcessed()
		{
			return Task.Run( () =>
			{
				try
				{
					InitProcMan();

					if( ProcMan.ProcList.Count == 0 )
					{
						throw new ArgumentNullException( "ProcList is empty" );
					}

					XParameter SParam = PSettings.Parameter( "ProcessState" );

					if ( SParam != null && ( ProcessSuccess = SParam.GetBool( "Success" ) ) )
					{
						BInst = new BookInstruction( ZoneId, ZItemId );
						Name = BInst.Title;

						Shared.BooksDb.LockAction( Db =>
						{
							Processed = Db.Entry( BInst.Entry ).IsKeySet && Db.Volumes.Any( x => x.Book == BInst.Entry );
						} );

						if( Processed )
						{
							Desc = new StringResBg().Text( "RecordExist" );
						}
					}

					CanProcess = true;
				}
				catch ( Exception ex )
				{
					Name = "ERROR";
					Desc = ex.Message;
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
				string OLocation = MetaLocation;
				string OZoneId = ZoneId;
				string OZItemId = ZItemId;

				ZoneId = AppKeys.ZLOCAL;
				ZItemId = Id;

				try
				{
					XParameter Param = PSettings.Parameter( "Procedures" );
					Param.SetValue( new XKey( "Guid", Id ) );
					PSettings.SetParameter( Param );

					// Move location
					PSettings.Location = MetaLocation;
					PSettings.Save();
					Shared.Storage.DeleteFile( OLocation );

					Book Entry = Shared.BooksDb.GetBook( OZoneId, OZItemId, BookType.S );
					Entry.ZoneId = AppKeys.ZLOCAL;
					Entry.ZItemId = Id;
					Shared.BooksDb.SaveBook( Entry );

					BInst = new BookInstruction( ZoneId, ZItemId );
				}
				catch ( Exception )
				{
					BInst = null;
					Processed = false;
					Logger.Log( ID, string.Format( "Failed to move SVol: {0} => {1}", OLocation, MetaLocation ), LogType.WARNING );
				}
			}
		}

		public async Task<SpiderBook> Clone()
		{
			// Get the latest settings from original location
			XRegistry OSettings = new XRegistry( "<ProcSpider />", PSettings.Location );

			// We only need the "Procedures" node
			// Which also truncates all runtime infomation store from previous session
			XParameter Param = OSettings.Parameter( "Procedures" );

			// Apply the new Guid to the script
			XRegistry NDef = new XRegistry( "<ProcSpider />", "", false );
			Param.SetValue( new XKey( "Guid", Guid.NewGuid() ) );
			NDef.SetParameter( Param );

			SpiderBook Bk = await ImportFile( NDef.ToString(), true );
			Bk.Name = Name + " - Copy";

			return Bk;
		}

		public BookInstruction GetBook() => BInst;

		override protected void MessageBus_OnDelivery( Message Mesg )
		{
			if ( !( Mesg.Payload is ProceduresPanel.PanelLog ) ) return;
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