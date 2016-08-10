using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace wenku8.Model.ListItem.Sharers
{
    using SHTarget = REST.SharersRequest.SHTarget;
    sealed class SHRequest : Comments.Comment
    {
        public string Id { get; set; }
        public string Pubkey { get; private set; }

        public string ScriptId { get; private set; }
        public string ScriptName { get; private set; }
        public SHTarget Target { get; private set; }

        private bool _Granted;
        public bool Granted
        {
            get { return _Granted; }
            set
            {
                _Granted = value;
                NotifyChanged( "Granted" );
            }
        }

        public SHRequest( JsonObject Def )
        {
            JsonObject JAuthor = Def.GetNamedObject( "author" );
            Username = JAuthor.GetNamedString( "name" );
            UserId = JAuthor.GetNamedString( "_id" );

            Id = Def.GetNamedString( "_id" );
            Title = Def.GetNamedString( "remarks" );
            Pubkey = Def.GetNamedString( "pubkey" );

            IJsonValue JValue;
            if ( Def.TryGetValue( "script", out JValue ) )
            {
                JsonObject JScript = JValue.GetObject();
                ScriptName = JScript.GetNamedString( "name" );
                ScriptId = JScript.GetNamedString( "_id" );
            }

            if ( Def.TryGetValue( "target", out JValue ) )
            {
                switch ( Def.GetString() )
                {
                    case "tok": Target = SHTarget.TOKEN; break;
                    case "key": Target = SHTarget.KEY; break;
                    default:
                        throw new Exception( "Invalid request target" );
                }
            }

            TimeStamp = DateTime.Parse( Def.GetNamedString( "date_created" ) );
        }
    }
}
