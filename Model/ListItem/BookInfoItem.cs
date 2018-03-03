using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Model.ListItem
{
	using Database.Models;
	using Model.Book;
	using Resources;
	using Settings;

	class BookBannerItem : ActiveItem
	{
		public BookItem BkItem { get; private set; }

		private string BannerUrl => FileLinks.ROOT_BANNER + BkItem.ZItemId;
		public bool BannerExist => Shared.Storage.FileExists( BannerUrl );
		public Uri UriSource => new Uri( "ms-appdata:///local/" + BannerUrl );

		public BookBannerItem( BookItem Bk, string Title, string Intro, string Date )
			: base( Title, Date, Intro, Bk.ZItemId )
		{
			BkItem = Bk;
		}

		public void SaveBanner( byte[] Data )
		{
			Shared.Storage.WriteBytes( BannerUrl, Data );
			NotifyChanged( "BannerExist" );
		}

	}
}