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
    using AuthManager = System.AuthManager;
    using CryptAES = System.CryptAES;

    sealed class HubScriptItem : ActiveItem
    {
        private bool _inCollection;

        public string Id { get { return Payload as string; } }
        public string Author { get; private set; }
        public string AuthorId { get; private set; }

        public bool InCollection
        {
            get { return _inCollection; }
            set
            {
                _inCollection = value;
                NotifyChanged( "InCollection" );
            }
        }

        public bool Encrypted { get; private set; }
        public bool ForceEncryption { get; private set; }

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
            Author = "";

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
            Encrypted = Def.GetNamedBoolean( "enc" );
            ForceEncryption = Def.GetNamedBoolean( "force_enc" );

            NotifyChanged( "Histories", "Error", "HistoryError", "Tags", "Zone", "Type", "Author", "Encrypted" );
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

            string Data = JResponse.GetNamedString( "data" );
            if ( Encrypted )
            {
                AuthManager AMgr = new AuthManager();
                CryptAES Crypt = AMgr.GetKeyById( Id );

                if ( Crypt != null )
                {
                    try
                    {
                        Data = Crypt.Decrypt( Data );
                    }
                    catch ( Exception )
                    {
                        // This will fallback to try all keys
                        Crypt = null;
                    }
                }

                if ( Crypt == null && !AMgr.TryDecrypt( Data, out Data ) )
                {
                    MessageBus.SendUI( new Message( typeof( HubScriptItem ), AppKeys.HS_DECRYPT_FAIL, this ) );
                    return;
                }
            }

            ScriptFile = await AppStorage.MkTemp();
            await ScriptFile.WriteString( Data );
            MessageBus.SendUI( new Message( typeof( HubScriptItem ), AppKeys.SH_SCRIPT_DATA, this ) );
        }

    }
}