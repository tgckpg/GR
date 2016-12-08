using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using Net.Astropenguin.Loaders;

namespace wenku8.AdvDM
{
	sealed class WHttpRequest : HttpRequest
	{
        public static CookieContainer Cookies = new CookieContainer();
        internal static string UA = "WHTTPRequest";

		public WHttpRequest( Uri RequestUri )
            :base( RequestUri )
		{
			WCacheMode.OfflineEnabled += WCacheMode_OfflineEnabled;
		}

        ~WHttpRequest()
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
            WCMessage.Method = HttpMethod.Post;
            UserAgent = UA;
            Kookies = Cookies;
		}
	}
}