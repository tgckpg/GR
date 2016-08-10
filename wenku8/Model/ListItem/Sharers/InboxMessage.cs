using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace wenku8.Model.ListItem.Sharers
{
    using AdvDM;
    using Resources;
    using System;

    sealed class InboxMessage : ActiveItem
    {
        public DateTime TimeStamp { get; private set; }

        public string ScriptId { get { return Desc; } }
        public string CommId { get { return Desc2; } }

        public NameValue<Action> Activity;

        public InboxMessage( JsonObject JDef )
        {
            JsonValue JLink = JDef.GetNamedValue( "link" );
            if ( JLink.ValueType == JsonValueType.String )
            {
                string[] S = JLink.GetString().Split( new char[] { ',' }, 3 );
                switch( S[0] )
                {
                    case "COMM":
                        Desc = S[ 1 ];
                        Desc2 = S[ 2 ];
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
            new RuntimeCache().POST(
                Shared.ShRequest.Server
                , Shared.ShRequest.MessageRead( Payload )
                , Utils.DoNothing
                , Utils.DoNothing
                , false
            );
        }
    }
}