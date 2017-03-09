using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace wenku8.Model.REST
{
	using AdvDM;

	sealed class BingImageSearch
	{
		private const string ImgQuery = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?q={0}&count=1&offset={1}&mkt=en-us&safeSearch=strict";

		private string q;

		public JsonObject ResultObj { get; private set; }

		public BingImageSearch( string q )
		{
			this.q = q;
		}

		public async Task<string> GetImage( int offset )
		{
			TaskCompletionSource<string> TCS = new TaskCompletionSource<string>();

			BingHttpRequest Request = new BingHttpRequest( new Uri( string.Format( ImgQuery, Uri.EscapeDataString( q ), offset ) ) );
			Request.OnRequestComplete += ( e ) =>
			{
				try
				{
					string ThumbUrl = GetThumbnailUrl( e.ResponseString );
					TCS.TrySetResult( ThumbUrl );
				}
				catch( Exception )
				{
					TCS.TrySetResult( null );
				}
			};

			Request.OpenAsync();
			return await TCS.Task;
		}

		private string GetThumbnailUrl( string ResponseStr )
		{
			JsonObject Obj = JsonObject.Parse( ResponseStr );
			JsonArray ResultArr = Obj.GetNamedArray( "value" );

			ResultObj = ResultArr.FirstOrDefault()?.GetObject();

			return ResultObj?.GetNamedString( "thumbnailUrl" );
		}

	}

}