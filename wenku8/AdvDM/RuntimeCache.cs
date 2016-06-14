using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

namespace wenku8.AdvDM
{
    using Settings;
    using Resources;

    class RuntimeCache
    {
        public static readonly string ID = typeof( RuntimeCache ).Name;

        public bool EN_UI_Thead = false;

        protected Func<Uri, HttpRequest> MakeRequest;

        public RuntimeCache()
        {
            MakeRequest = a => new WHTTPRequest( a ) { EN_UITHREAD = EN_UI_Thead };
        }

        public RuntimeCache( Func<Uri, HttpRequest> RequestMaker )
        {
            MakeRequest = RequestMaker;
        }

		virtual public void GET( Uri Guri
			, Action<DRequestCompletedEventArgs, string> Handler
			, Action<string, string, Exception> DownloadFailedHandler, bool precache )
		{
			Logger.Log( ID, "GET: " + Guri.ToString(), LogType.DEBUG );
			if ( WCacheMode.OfflineMode )
			{
				 DownloadFailedHandler( Guri.OriginalString, "", new Exception( "Currently offline" ) );
			}
			else
			{
				SendRequest( Guri, Handler, DownloadFailedHandler, precache );
			}
		}

        virtual protected void SendRequest( Uri Guri
        , Action<DRequestCompletedEventArgs, string> Handler
        , Action<string, string, Exception> DowloadFailedHandler, bool precache )
        {
            // Initialize HTTP Request
            HttpRequest wc = MakeRequest( Guri );
            wc.Method = "GET";

			// Download handler
			wc.OnRequestComplete += ( e ) => PreHandler(
				// cache name is just the uri
				Guri.OriginalString
				// When download success, these param will send to handler
				, e, Guri.OriginalString, Handler
				// Download failed handler
				, DowloadFailedHandler
				// this determine whether PreHandler should cache the downloaded stream
				, precache
            );
            // Start Request
            wc.OpenAsync();
		}

		virtual protected void PreHandler( string CacheName, DRequestCompletedEventArgs e, string id
			, Action<DRequestCompletedEventArgs, string> Handler
			, Action<string, string, Exception> DowloadFailedHandler, bool PreCache )
		{
			string cache = Uri.EscapeDataString( CacheName );
			try
			{
				string dString = e.ResponseString;

				// Write Cache
				if ( PreCache )
				{
                    // Precache is written in cache Using EscapedataString
                    Shared.Storage.WriteBytes( FileLinks.ROOT_CACHE + cache, e.ResponseBytes );
				}
			}
			catch ( Exception ex )
			{
                if( ex is WebException )
                {
                    Logger.Log( ID, "Non Web Exception Occured: " + ex.Message, LogType.ERROR );
                }

                // Logger.Log( ID, e.ResponseString, LogType.DEBUG );
                // Return with the generated cache name for futher operation
                DowloadFailedHandler( cache, id, ex );
                return;
			}

            // This should not be catched inside the try catch above
            // Since it will be redirect to download failed handler
            Handler( e, id );
		}
    }
}
