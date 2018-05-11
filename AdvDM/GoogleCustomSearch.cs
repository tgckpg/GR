using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.AdvDM
{
	using Model.Book;
	using Model.Interfaces;
	using Model.REST;
	using Config;

	sealed class GoogleCustomSearch : IImageService
	{
		public string DefaultKeyword => Book.Title + " 1";
		private BookItem Book;

		private GoogleImageSearch.ImageItem CurrentItem;

		public GoogleCustomSearch( BookItem Bk )
		{
			Book = Bk;
		}

		public bool Exists() => true;

		public async Task<string> GetImage( bool FullSize = false )
		{
			GoogleImageSearch BReq = new GoogleImageSearch( GetKeyword() );
			if ( !( Book.Entry.Meta.TryGetValue( "ImageSearch:offset", out string sOffset ) && int.TryParse( sOffset, out int Offset ) ) )
			{
				Offset = 0;
			}

			CurrentItem = await BReq.GetImage( Offset );
			return CurrentItem?.Link;
		}

		public string GetKeyword()
		{
			if ( !Book.Entry.Meta.TryGetValue( "ImageSearch", out string Keyword ) )
			{
				Keyword = DefaultKeyword;
			}

			return Keyword;
		}

		public async Task<string> GetSearchQuery()
		{
			await GetImage();
			return CurrentItem?.ContextLink;
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

		public void SetApiKey( string ApiKey )
		{
			GRConfig.System.GCustomSearchAPI = ApiKey;
		}
	}
}