using System;
using System.IO;
using System.Text;
using System.Net;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Loaders;

namespace wenku8.AdvDM
{
	sealed class WHTTPRequest : HttpRequest
	{
        public static CookieContainer Cookies = new CookieContainer();
        internal static string UA = "WHTTPRequest";

		public WHTTPRequest( Uri RequestUri )
            :base( RequestUri )
		{
			WCacheMode.OfflineEnabled += WCacheMode_OfflineEnabled;
		}

        ~WHTTPRequest()
        {
			WCacheMode.OfflineEnabled -= WCacheMode_OfflineEnabled;
        }

		void WCacheMode_OfflineEnabled()
		{
			Stop();
			WCacheMode.OfflineEnabled -= WCacheMode_OfflineEnabled;
		}

		override protected void CreateRequest()
		{
            base.CreateRequest();
			WCRequest.Method = "POST";
            WCRequest.Headers[ HttpRequestHeader.UserAgent ] = UA;

            if ( WCRequest.SupportsCookieContainer )
            {
                WCRequest.CookieContainer = Cookies;
            }
		}
	}
}