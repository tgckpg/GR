using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

namespace wenku8.Model.ListItem.Sharers
{
    using AdvDM;
    using REST;
    using Resources;
    using Settings;
    using System;

    sealed class InboxMessage : ActiveItem
    {
        public static readonly string ID = typeof( InboxMessage ).Name;

        public DateTime TimeStamp { get; private set; }

        public string ScriptId { get { return Desc; } }
        public string CommId { get { return Desc2; } }

        public HubScriptItem HubScript { get; private set; }
        public NameValue<Action> Activity;

        public InboxMessage( JsonObject JDef )
        {
            JsonValue JLink = JDef.GetNamedValue( "link" );
            if ( JLink.ValueType == JsonValueType.String )
            {
                string[] DefId = JLink.GetString().Split( new char[] { ',' }, 3 );
                switch( DefId[0] )
                {
                    case "COMM":
                        Desc = DefId[ 1 ];
                        Desc2 = DefId[ 2 ];
                        break;
                }
            }

            Name = JDef.GetNamedString( "mesg" );
            TimeStamp = DateTime.Parse( JDef.GetNamedString( "date" ) );
            Payload = JDef.GetNamedString( "id" );

            Activity = new NameValue<Action>( Name, OpenComment );
        }

        private void OpenComment()
        {
            RuntimeCache RCache = new RuntimeCache();

            // Flag message as read
            // This will remove the message from inbox on the server
            RCache.POST(
                Shared.ShRequest.Server
                , Shared.ShRequest.MessageRead( Payload )
                , Utils.DoNothing
                , Utils.DoNothing
                , false
            );

            RCache.POST(
                Shared.ShRequest.Server
                , Shared.ShRequest.Search( "uuid: " + ScriptId, 0, 1 )
                , ( e, Id ) =>
                {
                    try
                    {
                        JsonObject JDef = JsonStatus.Parse( e.ResponseString );
                        HubScript = new HubScriptItem( JDef.GetNamedArray( "data" ).First().GetObject() );
                        MessageBus.SendUI( GetType(), AppKeys.HS_OPEN_COMMENT, this );
                    }
                    catch( Exception ex )
                    {
                        Logger.Log( ID, ex.Message, LogType.WARNING );
                    }
                }
                , ( cache, q, ex ) =>
                {
                    Logger.Log( ID, ex.Message, LogType.WARNING );
                }
                , true
            );
        }
    }
}