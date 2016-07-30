using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.DataModel;

namespace wenku8.Model.ListItem
{
    using SHTarget = REST.SharersRequest.SHTarget;

    sealed class SHGrant : ActiveData
    {
        public string Id { get; set; }
        public string[] Grants { get; private set; }

        public string ScriptId { get; private set; }
        public string ScriptName { get; private set; }
        public SHTarget Target { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public SHGrant( JsonObject Def )
        {
            Id = Def.GetNamedString( "_id" );

            IJsonValue JValue;
            if ( Def.TryGetValue( "script", out JValue ) )
            {
                JsonObject JScript = JValue.GetObject();
                ScriptName = JScript.GetNamedString( "name" );
                ScriptId = JScript.GetNamedString( "uuid" );
            }

            if ( Def.TryGetValue( "target", out JValue ) )
            {
                switch ( JValue.GetString() )
                {
                    case "token_requests": Target = SHTarget.TOKEN; break;
                    case "key_requests": Target = SHTarget.KEY; break;
                    default:
                        throw new Exception( "Invalid request target" );
                }
            }

            if( Def.TryGetValue( "grants", out JValue ) )
            {
                JsonArray JGrants = JValue.GetArray();
                List<string> GrantList = new List<string>( JGrants.Count() );
                foreach( JsonValue JGrant in JGrants )
                {
                    GrantList.Add( JGrant.GetString() );
                }

                Grants = GrantList.ToArray();
            }

            TimeStamp = DateTime.Parse( Def.GetNamedString( "date_created" ) );
        }
    }
}