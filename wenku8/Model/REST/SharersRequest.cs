using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Model.REST
{
    using AdvDM;
    using Config;

    sealed class SharersRequest
    {
        public Uri Server = new Uri( "http://w10srv.botanical.astropenguin.net/" );

        private readonly string LANG = Properties.LANGUAGE;

        public enum StatusType
        {
            INVALID_SCRIPT = -1
        }

        public enum SHTarget : byte { SCRIPT = 1, COMMENT = 2, KEY = 4, TOKEN = 8 }

        public PostData ReserveId( string AccessToken )
        {
            return new PostData(
                "SHHUB_RESERVE_UUID"
                , Compost(
                    "action", "reserve-uuid"
                    , "access_token", AccessToken
                )
            );
        }

        public PostData GetComments( SHTarget Target, int Skip, uint Limit, params string[] Ids )
        {
            List<string> Params = new List<string>( new string[]
            {
                "action", "get-comment"
                , "skip", Skip.ToString()
                , "limit", Limit.ToString()
                , "target", ( Target ^ SHTarget.COMMENT ) == 0 ? "comment" : "script"
            } );

            foreach ( string Id in Ids )
            {
                Params.Add( "id" );
                Params.Add( Id );
            }

            return new PostData( "SH_GET_COMMENTS", Compost( Params.ToArray() ) );
        }

        public PostData Comment( SHTarget Target, string Id, string Content, bool Encrypted )
        {
            return new PostData( "SH_POST_COMMENT", Compost(
                "action", "comment"
                , "id", Id
                , "content", Content
                , "enc", Encrypted ? "1" : "0"
                , "target", ( Target ^ SHTarget.COMMENT ) == 0 ? "comment" : "script"
            ) );
        }

        public PostData ScriptUpload(
            string AccessToken, string Id, string ScriptData
            , string Name, string Desc
            , string Zone, string[] Types, string[] Tags
            , bool Encrypted, bool ForceEncrypt, bool Anon )
        {
            List<string> Params = new List<string>( new string[] {
                "action", "upload"
                , "uuid", Id
                , "data", ScriptData
                , "name", Name
                , "desc", Desc
                , "access_token", AccessToken
            } );

            if ( Encrypted )
            {
                Params.Add( "enc" );
                Params.Add( "1" );
            }

            if( ForceEncrypt )
            {
                Params.Add( "force_enc" );
                Params.Add( "1" );
            }

            if( Anon )
            {
                Params.Add( "anon" );
                Params.Add( "1" );
            }

            ZoneTypeTags( Params, new string[] { Zone }, Types, Tags );

            return new PostData( Id, Compost( Params.ToArray() ) );
        }

        public PostData ScriptDownload( string Id, string AccessToken )
        {
            return new PostData(
                Id, Compost(
                    "action", "download"
                    , "uuid", Id
                    , "access_token", string.IsNullOrEmpty( AccessToken ) ? "" : AccessToken
                )
            );
        }

        public PostData StatusReport( string Id, string StatusType, string Desc = "" )
        {
            return new PostData(
                Id, Compost(
                    "action", "status-report"
                    , "uuid", Id
                    , "type", StatusType
                    , "desc", Desc
                )
            );
        }

        public PostData ScriptRemove( string AccessToken, string Id )
        {
            return new PostData(
                Id, Compost(
                    "action", "remove"
                    , "uuid", Id
                    , "access_token", AccessToken
                )
            );
        }

        public PostData Register( string Username, string Passwd, string Email )
        {
            return new PostData(
                "SHHUB_REGISTER"
                , Compost(
                    "action", "register"
                    , "user", Username
                    , "passwd", Passwd
                    , "email", Email
                )
            );
        }

        public PostData PlaceRequest( SHTarget Target, string PubKey, string Id, string Remarks )
        {
            string ParamTarget = ( Target ^ SHTarget.KEY ) == 0 ? "key" : "token";
            return new PostData(
                "KEY_REQUEST: " + ParamTarget
                , Compost(
                    "action", "place-request"
                    , "id", Id
                    , "target", ParamTarget
                    , "remarks", Remarks
                    , "pubkey", PubKey
                )
            );
        }

        public PostData GrantRequest( string Id, string Grant )
        {
            return new PostData(
                "GRANT_REQUEST: " + Id
                , Compost(
                    "action", "grant-request"
                    , "grant", Grant
                )
            );
        }

        public PostData GetRequests( SHTarget Target, string Id, int Skip, uint Limit )
        {
            string ParamTarget = ( Target ^ SHTarget.KEY ) == 0 ? "key" : "token";
            return new PostData(
                "SH_GET_REQUEST: " + ParamTarget
                , Compost(
                    "action", "get-requests"
                    , "id", Id
                    , "skip", Skip.ToString()
                    , "limit", Limit.ToString()
                    , "target", ParamTarget
                )
            );
        }

        public PostData MyRequests()
        {
            return new PostData(
                "SH_MY_REQUESTS"
                , Compost( "action", "my-requests" )
            );
        }

        public PostData Login( string Username, string Passwd )
        {
            return new PostData(
                "SHHUB_LOGIN"
                , Compost(
                    "action", "login"
                    , "user", Username
                    , "passwd", Passwd
                )
            );
        }

        public PostData Logout()
        {
            return new PostData( "SHHUB_LOGOUT", Compost( "action", "logout" ) );
        }

        public PostData SessionValid()
        {
            return new PostData( "SHHUB_VALIDATE_SESS", Compost( "action", "session-valid" ) );
        }

        public PostData Search( string Query, int Skip, uint Limit, IEnumerable<string> AccessTokens = null )
        {
            /**
             * Here is how the query is parsed
             * Split ':' into groups
             *   <-: is the property to filter
             *   :-> is the filter value
             **/

            string[] QString = Query.Split( ':' );
            int l = QString.Length - 1;

            List<string> Queries = new List<string>( new string[] {
                "action", "search"
                , "skip", Skip.ToString()
                , "limit", Limit.ToString()
            } );

            if( AccessTokens != null )
            foreach( string AccessToken in AccessTokens )
            {
                Queries.Add( "access_token" );
                Queries.Add( AccessToken );
            }

            // Default searches this script name
            if ( l < 1 ) return new PostData( "SHHUB_SEARCH", Compost( Queries.ToArray() ) );

            QLoop:
            for ( int i = 0; i < l; i++ )
            {
                string QName = QString[ i ];

                string PropFilter = QName.Substring( QName.LastIndexOf( ' ' ) + 1 );
                string FilterVal = QString[ i + 1 ].Trim();

                if ( i + 2 <= l )
                {
                    FilterVal = FilterVal.Substring( 0, FilterVal.LastIndexOf( ' ' ) );
                }
                switch ( PropFilter )
                {
                    case "tag":
                        PropFilter += "s";
                        break;
                    case "zones":
                    case "types":
                        PropFilter = PropFilter.Substring( 0, PropFilter.Length - 1 );
                        break;
                    // Page params should be filtered
                    case "skip": case "limit":
                        goto QLoop;
                }

                Queries.Add( PropFilter );
                Queries.Add( FilterVal );
            }

            if ( 2 < Queries.Count() )
            {
                int NameIndex = QString[ 0 ].LastIndexOf( ' ' );
                if ( ~NameIndex != 0 )
                {
                    Queries.Add( "name" );
                    Queries.Add( QString[ 0 ].Substring( 0, NameIndex ).Trim() );
                }
            }

            return new PostData( "SHHUB_SEARCH", Compost( Queries.ToArray() ) );
        }

        private string Compost( params string[] Pairs )
        {
            int l = Pairs.Length;
#if DEBUG
            if ( l % 2 != 0 || l == 0 )
            {
                throw new ArgumentException( "Arguments does not seems to be in pairs" );
            }
#endif
            string Composted = "lang=" + LANG + "&t=" + DateTime.UtcNow.Ticks.ToString();

            for ( int i = 0; i < l; i++ )
            {
                Composted += ( i % 2 == 0 ? "&" : "=" ) + Uri.EscapeDataString( Pairs[ i ] );
            }

            return Composted;
        }

        private void ZoneTypeTags( IList<string> Params, string[] Zones, string[] Types, string[] Tags )
        {
            if( Zones != null )
            foreach ( string Zone in Zones )
            {
                Params.Add( "zone" );
                Params.Add( Zone );
            }

            if( Types != null )
            foreach ( string Type in Types )
            {
                Params.Add( "type" );
                Params.Add( Type );
            }

            if( Tags != null )
            foreach ( string Tag in Tags )
            {
                Params.Add( "tags" );
                Params.Add( Tag );
            }
        }

    }
}