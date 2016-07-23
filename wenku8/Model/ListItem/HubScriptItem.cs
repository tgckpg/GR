using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Data.Json;
using Windows.Storage;

using Net.Astropenguin.IO;
using Net.Astropenguin.Linq;
using Net.Astropenguin.Messaging;

namespace wenku8.Model.ListItem
{
    using Settings;

    sealed class HubScriptItem : ActiveItem
    {
        public string Id { get { return Payload as string; } }
        public string Author { get; private set; }
        public string AuthorId { get; private set; }

        public IEnumerable<string> Tags { get; private set; }
        public IEnumerable<string> Type { get; private set; }
        public IEnumerable<string> Zone { get; private set; }
        public IEnumerable<string> ZoneTypeTags
        {
            get
            {
                return new string[] { Zone.FirstOrDefault(), Type.FirstOrDefault(), Tags.FirstOrDefault() }.Where( x => x != null );
            }
        }

        public IEnumerable<HubScriptStatus> Histories { get; private set; }

        public bool Error
        {
            get
            {
                int? Status = Histories?.FirstOrDefault()?.Status;
                if ( Status != null ) return Status < 0;

                return false;
            }
        }

        public string HistoryError { get; private set; }
        public string ErrorMessage
        {
            get { return Desc2; }
            set
            {
                Desc2 = value;
                NotifyChanged( "ErrorMessage" );
            }
        }

        public IStorageFile ScriptFile { get; private set; }

        public HubScriptItem( JsonObject Def )
            : base( Def.GetNamedString( "name" ), Def.GetNamedString( "desc" ), Def.GetNamedString( "uuid" ) )
        {
            Update( Def );
        }

        public void Update( JsonObject Def )
        {
            Author = "Anonymous";

            if ( Def.GetNamedValue( "author" ).ValueType == JsonValueType.Object )
            {
                JsonObject JAuthor = Def.GetNamedObject( "author" );
                Author = JAuthor.GetNamedString( "display_name" );
                AuthorId = JAuthor.GetNamedString( "_id" );
            }

            if ( Def.GetNamedArray( "history" ).ValueType == JsonValueType.Array )
            {
                try
                {
                    JsonArray HistDef = Def.GetNamedArray( "history" );

                    int i = 0;
                    int l = HistDef.Count();
                    HubScriptStatus[] HistStatus = new HubScriptStatus[ l-- ];

                    foreach ( JsonValue Item in HistDef )
                    {
                        HistStatus[ l - ( i++ ) ] = new HubScriptStatus( Item.GetObject() );
                    }

                    Histories = HistStatus;
                }
                catch( Exception ex )
                {
                    HistoryError = ex.Message;
                }
            }

            Tags = Def.GetNamedArray( "tags" ).Remap( x => x.GetString() );
            Zone = Def.GetNamedArray( "zone" ).Remap( x => x.GetString() );
            Type = Def.GetNamedArray( "type" ).Remap( x => x.GetString() );

            NotifyChanged( "Histories", "Error", "HistoryError", "Tags", "Zone", "Type", "Author" );
        }

        public async void SetScriptData( string JsonData )
        {
            JsonObject JResponse;

            if ( !JsonObject.TryParse( JsonData, out JResponse ) )
            {
                ErrorMessage = "A server Error has occurred";
                return;
            }

            if ( !JResponse.GetNamedBoolean( "status", false ) )
            {
                ErrorMessage = JResponse.GetNamedString( "message" );
                return;
            }

            ScriptFile = await AppStorage.MkTemp();
            await ScriptFile.WriteString( JResponse.GetNamedString( "data" ) );

            MessageBus.SendUI( new Message( typeof( HubScriptItem ), AppKeys.SH_SCRIPT_DATA, this ) );
        }

    }
}