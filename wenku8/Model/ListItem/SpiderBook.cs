using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

using libtaotu.Pages;
using libtaotu.Controls;
using libtaotu.Crawler;
using libtaotu.Models.Procedure;

namespace wenku8.Model.ListItem
{
    using Book.Spider;
    using Resources;
    using Settings;

    sealed class SpiderBook : LocalBook
    {
        private ProcManager ProcMan;
        private BookInstruction BInst;
        public XRegistry PSettings { get; private set; }
        public bool IsSpider { get { return true; } }

        public string MetaRoot { get { return FileLinks.ROOT_SPIDER_VOL + aid + "/"; } } 
        public string MetaLocation { get { return MetaRoot + "METADATA.xml"; } }

        private SpiderBook() { }

        private void InitProcMan()
        {
            if ( ProcMan != null ) return;
            ProcMan = new ProcManager();
            XParameter Param = PSettings.Parameter( "Procedures" );
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

        public static async Task<SpiderBook> CreateAsyncSpider( string Id )
        {
            SpiderBook Book = new SpiderBook();
            Book.aid = Id;
            Book.PSettings = new XRegistry( "<ProcSpider />", Book.MetaLocation );

            await Book.TestProcessed();
            return Book;
        }

        protected override async Task TestProcessed()
        {
            Name = "Spider Script";
            Desc = "Click to crawl";

            await Task.Run( () =>
            {
                try
                {
                    InitProcMan();
                    XParameter SParam = PSettings.Parameter( "ProcessState" );

                    BInst = new BookInstruction( aid, PSettings );
                    if ( SParam != null
                        && ( ProcessSuccess = SParam.GetBool( "Success" ) ) )
                    {
                        Worker.UIInvoke( () =>
                        {
                            Name = BInst.Title;
                            Desc = BInst.RecentUpdate;
                        } );
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

            ProcConvoy Convoy = null;

            if( BInst == null )
            {
                Convoy = await Spider.Crawl();
            }
            else
            {
                ProcPassThru PThru = new ProcPassThru( null );
                Convoy = await Spider.Crawl( new ProcConvoy( PThru, BInst ) );
            }

            Convoy = ProcManager.TracePackage( Convoy, ( D, C ) => C.Payload is BookInstruction );

            if( Convoy == null ) throw new Exception( "Unable to find Book Info" );

            BInst = Convoy.Payload as BookInstruction;
            BInst.SaveInfo( PSettings );

            Name = BInst.Title;
            Desc = BInst.RecentUpdate;

            XParameter XParam = new XParameter( "ProcessState" );
            XParam.SetValue( new XKey( "Success", true ) );

            PSettings.SetParameter( XParam );
            PSettings.Save();

            ProcessSuccess = true;
        }

        public void AssignId( string Id )
        {
            if ( ProcMan.GUID == Id ) return;

            ProcMan.GUID = Id;
            if ( ProcMan.GUID == Id )
            {
                string OldRoot = MetaRoot;

                aid = Id;
                XParameter Param = PSettings.Parameter( "Procedures" );
                Param.SetValue( new XKey( "Guid", Id ) );
                PSettings.SetParameter( Param );

                // Begin Move location
                PSettings.Location = MetaLocation;
                try
                {
                    Shared.Storage.MoveDir( OldRoot, MetaRoot );
                }
                catch ( Exception )
                {
                    Logger.Log( ID, string.Format( "Failed to move SVol: {0} => {1}", OldRoot, MetaRoot ), LogType.WARNING );
                }

                PSettings.Save();
            }
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

            Desc = "Script is not available";
        }
    }
}