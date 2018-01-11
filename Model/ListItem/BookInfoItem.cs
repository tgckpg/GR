using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.Helpers;

namespace GR.Model.ListItem
{
	using GR.Settings;
	using Resources;

	class BookBannerItem : ActiveItem
	{
		public int BookId;

		private string BannerUrl => FileLinks.ROOT_BANNER + BookId;
		public bool BannerExist => Shared.Storage.FileExists( BannerUrl );
		public Uri UriSource => new Uri( "ms-appdata:///local/" + BannerUrl );

		public BookBannerItem( int BookId, string Title, string Intro, string Date )
			: base( Title, Date, Intro, BookId.ToString() )
		{
			this.BookId = BookId;
		}

		public void SaveBanner( byte[] Data )
		{
			Shared.Storage.WriteBytes( BannerUrl, Data );
			NotifyChanged( "BannerExist" );
		}

	}
}