using System;
using System.IO;
using System.Text;
using System.Net;

using Net.Astropenguin.Loaders;

namespace wenku8.AdvDM
{
	sealed class SHttpRequest : HttpRequest
	{
		public SHttpRequest( Uri RequestUri )
            :base( RequestUri ) { }

		override protected void CreateRequest()
		{
            base.CreateRequest();
            WCRequest.Headers[ HttpRequestHeader.UserAgent ] = WHttpRequest.UA;
		}
	}
}