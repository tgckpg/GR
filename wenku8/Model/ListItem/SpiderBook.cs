using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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

    class SpiderBook : LocalBook
    {
        private ProcManager ProcMan;
        private BookInstruction BInst;
        public XRegistry PSettings { get; private set; }
        public bool IsSpider { get { return true; } }

        public string MetaLocation
        {
            get { return FileLinks.ROOT_SPIDER_VOL + aid + "/METADATA.xml"; }
        }

        public SpiderBook( string id )
        {
            aid = id;
            PSettings = new XRegistry( "<ProcSpider />", MetaLocation );
            TestProcessed();
        }

        public SpiderBook( BookInstruction BInst )
            :this( BInst.Id )
        {
            this.BInst = BInst;
        }

        protected SpiderBook( string ProcSetting, bool Test )
        {
            PSettings = new XRegistry( ProcSetting, null );
            XParameter Param = PSettings.GetParameters().First();

            if( Test )
            {
                TestProcessed();
                if ( CanProcess || ProcessSuccess )
                {
                    PSettings.Location = MetaLocation;
                    PSettings.Save();
                }
            }
        }

        public static async Task<SpiderBook> CreateAsnyc( string ProcSetting, bool Test )
        {
            return await Task.Run( () =>
            {
                return new SpiderBook( ProcSetting, Test );
            } );
        }

        protected override void TestProcessed()
        {
            Name = "Spider Script";
            Desc = "Click to crawl";

            try
            {
                ProcMan = new ProcManager();
                XParameter Param = PSettings.GetParameter( "Procedures" );
                ProcMan.ReadParam( Param );

                aid = ProcMan.GUID.ToString();
                XParameter SParam = PSettings.GetParameter( "ProcessState" );

                BInst = new BookInstruction( aid, PSettings );
                if ( SParam != null
                    && ( ProcessSuccess = SParam.GetBool( "Success" ) ) )
                {
                    Name = BInst.Title;
                    Desc = BInst.RecentUpdate;
                }

                CanProcess = true;
            }
            catch( Exception ex )
            {
                Name = ex.Message;
                Desc = "ERROR";
                CanProcess = false;
                ProcessSuccess = false;
            }
        }

        protected override async Task Run()
        {

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
