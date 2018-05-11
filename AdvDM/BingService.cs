using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;

namespace GR.AdvDM
{
	using Config;
	using Model.Book;
	using Model.Interfaces;
	using Model.REST;

	sealed class BingService : IImageService
	{
		public static string API_KEY { get; set; }

		public string DefaultKeyword => Book.Title + " " + Book.Info.Author;

		private BookItem Book;

		public BingService( BookItem Book )
		{
			this.Book = Book;
		}

		public async Task<string> GetImage( bool FullSize = false )
		{
			BingImageSearch BReq = new BingImageSearch( GetKeyword() );

			if ( !( Book.Entry.Meta.TryGetValue( "ImageSearch:offset", out string sOffset ) && int.TryParse( sOffset, out int Offset ) ) )
			{
				Offset = 0;
			}

			string ImgUrl = await ( FullSize ? BReq.GetFullImage( Offset ) : BReq.GetImage( Offset ) );
			return ImgUrl;
		}

		public Task<string> GetSearchQuery()
		{
			Func<string> _Null = () => null;
			return Task.Run( _Null );
		}

		public string GetKeyword()
		{
			if ( !Book.Entry.Meta.TryGetValue( "ImageSearch", out string Keyword ) )
			{
				Keyword = DefaultKeyword;
			}

			return Keyword;
		}

		public void SetKeyword( string Keyword )
		{
			Book.Entry.Meta[ "ImageSearch" ] = Keyword;
			Book.SaveInfo();
		}

		public void SetOffset( int Value )
		{
			if ( Book.Entry.Meta.TryGetValue( "ImageSearch:offset", out string sOffset ) && int.TryParse( sOffset, out int Offset ) )
			{
				Offset = Offset + Value;
			}
			else
			{
				Offset = 0;
			}

			if ( Offset < 0 ) Offset = 0;

			Book.Entry.Meta[ "ImageSearch:offset" ] = Offset.ToString();
		}

		public bool Exists() => false;

		public void SetApiKey( string Key )
		{
			GRConfig.System.BingImageAPI = Key;
			API_KEY = Key;
		}
	}

	sealed class BingHttpRequest : HttpRequest
	{
		public BingHttpRequest( Uri RequestUri ) : base( RequestUri ) { EN_UITHREAD = false; }

		override protected void CreateRequest()
		{
			base.CreateRequest();
			UserAgent = WHttpRequest.UA;
			WCMessage.Headers.Add( "Ocp-Apim-Subscription-Key", BingService.API_KEY );
		}
	}

}