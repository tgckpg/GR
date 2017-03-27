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

namespace wenku8.Model.ListItem
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

		public string MetaRoot { get { return FileLinks.ROOT_SPIDER_VOL + aid + "/"; } } 
		public string MetaLocation { get { return MetaRoot + "METADATA.xml"; } }

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

			aid = ProcMan.GUID;
		}

		public SpiderBook( BookInstruction BInst )
		{
			this.BInst = BInst;
			aid = BInst.Id;

			CanProcess = true;

			PSettings = new XRegistry( "<ProcSpider />", MetaLocation );
			InitProcMan();

			XParameter SParam = PSettings.Parameter( "ProcessState" );
			if ( SParam != null )
			{
				ProcessSuccess = SParam.GetBool( "Success" );
				HasChakra = SParam.GetBool( "HasChakra" );
			}
		}

		public static async Task<SpiderBook> ImportFile( string ProcSetting, bool Save )
		{
			SpiderBook Book = new SpiderBook();
			Book.PSettings = new XRegistry( ProcSetting, null );

			await Book.TestProcessed();
			if ( Book.CanProcess || Book.ProcessSuccess )
			{
				Book.PSettings.Location = Book.MetaLocation;
				if( Save ) Book.PSettings.Save();
			}

			return Book;
		}

		public static async Task<SpiderBook> CreateFromZoneInst( BookInstruction BInst )
		{
			SpiderBook Book = new SpiderBook();

			Book.aid = BInst.Id;
			Book.PSettings = new XRegistry( "<ProcSpider />", Book.MetaLocation, false );
			Book.PSettings.SetParameter( BInst.BookSpiderDef );

			BInst.SaveInfo( Book.PSettings );
			await Book.TestProcessed();

			return Book;
		}

		public static async Task<SpiderBook> CreateAsyncSpider( string Id )
		{
			SpiderBook Book = new SpiderBook();
			Book.aid = Id;
			Book.PSettings = new XRegistry( "<ProcSpider />", Book.MetaLocation );

			await Book.TestProcessed();
			return Book;
		}

		public override async Task Reload()
		{
			ProcMan = null;
			PSettings = new XRegistry( "<ProcSpider />", MetaLocation );
			ProcParameter.DestroyParams( PSettings );

			if ( BInst != null ) Shared.Storage.DeleteFile( BInst.CoverPath );

			await TestProcessed();
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

					BInst = new BookInstruction( aid, PSettings );
					if ( SParam != null
						&& ( ProcessSuccess = SParam.GetBool( "Success" ) ) )
					{
						Name = BInst.Title;
						Desc = BInst.RecentUpdate;
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

			if( BInst == null )
			{
				Convoy = await Spider.Crawl( Convoy );
			}
			else
			{
				BInst.Clear();

				ProcPassThru PThru = new ProcPassThru( Convoy );

				// SId is the parameter set by ZoneSpider
				if ( !string.IsNullOrEmpty( BInst.SId ) )
				{
					// Wrap another level
					PThru = new ProcPassThru( new ProcConvoy( PThru, BInst.SId ) );
				}

				Convoy = await Spider.Crawl( new ProcConvoy( PThru, BInst ) );
			}

			HasChakra = ProcManager.TracePackage( Convoy, ( D, C ) => D is ProcChakra ) != null;

			ProcParameter.StoreParams( Convoy, PSettings );
			Convoy = ProcManager.TracePackage( Convoy, ( D, C ) => C.Payload is BookInstruction );

			if( Convoy == null ) throw new Exception( "Unable to find Book Info" );

			BInst = ( BookInstruction ) Convoy.Payload;

			Name = BInst.Title;
			Desc = BInst.RecentUpdate;

			XParameter XParam = new XParameter( "ProcessState" );
			XParam.SetValue( new XKey[] {
				new XKey( "Success", true )
				, new XKey( "HasChakra", HasChakra )
			} );

			PSettings.SetParameter( XParam );
			BInst.SaveInfo( PSettings );
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

				string OId = aid;
				aid = Id;

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

					BInst = new BookInstruction( Id, PSettings );

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

			Param.SetValue( new XKey( "Guid", aid ) );
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
			if ( Shared.Storage.DirExist( FileLinks.ROOT_SPIDER_VOL + aid ) )
			{
				Shared.Storage.RemoveDir( FileLinks.ROOT_SPIDER_VOL + aid );
			}

			Processed = false;
			ProcessSuccess = false;
			CanProcess = false;

			Desc = "Script is unavailable";
		}
	}
}