using System;
using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;

namespace GR.Ext
{
	internal interface IRuntimeCache
	{
		void InitDownload(
			string id
			, XKey[] RequestKeys
			, Action<DRequestCompletedEventArgs, string> Complete
			, Action<string, string, Exception> Failed
			, bool useCache );

		void GET( Uri Guri
			, Action<DRequestCompletedEventArgs, string> Handler
			, Action<string, string, Exception> DownloadFailedHandler, bool precache );
	}
}