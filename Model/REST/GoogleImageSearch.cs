using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace GR.Model.REST
{
	using AdvDM;
	using GSystem;
	using Resources;

	sealed class GoogleImageSearch
	{
		public static string API_KEY { get; private set; }

		private const string ImgQuery = "https://www.googleapis.com/customsearch/v1?{0}&q={1}&fileType=jpg&imgSize=medium&searchType=image";

		private string q;

		public JsonObject ResultObj { get; private set; }

		public GoogleImageSearch( string q )
		{
			this.q = q;
		}

		public async Task<ImageItem> GetImage( int offset )
		{
			string GSearchId = "GImage." + Utils.Md5( q );
			string CachedJson = Shared.ZCacheDb.GetCache( GSearchId )?.Data.StringValue;

			if ( CachedJson == null )
			{

				TaskCompletionSource<ImageItem> TCS = new TaskCompletionSource<ImageItem>();
				WHttpRequest Request = new WHttpRequest( new Uri( string.Format( ImgQuery, API_KEY, Uri.EscapeDataString( q ) ) ) );
				Request.Method = HttpMethod.Get;
				Request.OnRequestComplete += ( e ) =>
				{
					try
					{
						Shared.ZCacheDb.Write( GSearchId, e.ResponseBytes );
						TCS.TrySetResult( GetImageItem( e.ResponseString, offset ) );
					}
					catch ( Exception )
					{
						TCS.TrySetResult( null );
					}
				};

				Request.OpenAsync();
				return await TCS.Task;
			}
			else
			{
				try
				{
					return GetImageItem( CachedJson, offset );
				}
				catch ( Exception )
				{
					return null;
				}
			}
		}

		private ImageItem GetImageItem( string ResponseStr, int offset )
		{
			JsonObject Obj = JsonObject.Parse( ResponseStr );
			JsonArray ResultArr = Obj.GetNamedArray( "items" );

			ResultObj = ResultArr[ offset ].GetObject();

			return new ImageItem( ResultObj );
		}

		public class ImageItem
		{
			public string Link { get; private set; }
			public int Width { get; private set; }
			public int Height { get; private set; }

			public string ContextLink { get; private set; }

			public ImageItem( JsonObject Item )
			{
				JsonObject ImageObj = Item.GetNamedObject( "image" );
				ContextLink = ImageObj?.GetNamedString( "contextLink" );

				Link = Item.GetNamedString( "link" );
				Width = ( int ) ImageObj.GetNamedNumber( "width" );
				Height = ( int ) ImageObj.GetNamedNumber( "height" );
			}
		}

	}
}