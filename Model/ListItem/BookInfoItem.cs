using System;

using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.Helpers;

namespace GR.Model.ListItem
{
	using Resources;

	public enum SectionMode
	{
		InfoPane, DirectNavigation
	}

	class BookInfoItem : ActiveItem
	{
		public SectionMode Mode = SectionMode.InfoPane;

		private string Path;

		public string SrcPath
		{
			get { return Path; }
			set
			{
				Path = value;
				AwaitBitmapSource();
			}
		}

		private ImageSource _banner;
		public ImageSource Banner { get { return _banner; } }

		public BookInfoItem( string aid, string Title, string Intro, string Date, string BannerPath )
			: base( Title, Date, Intro, aid )
		{
			SrcPath = BannerPath;
		}

		public BookInfoItem( string aid, string Title, string Intro, string Date )
			: base( Title, Date, Intro, aid )
		{
		}

		private async void AwaitBitmapSource()
		{
			if ( string.IsNullOrEmpty( Path ) || !Shared.Storage.FileExists( Path ) )
			{
				_banner = await Image.NewBitmap( new Uri( "ms-appx:///Assets/Samples/bookcoversample.png", UriKind.Absolute ) );
				NotifyChanged( "Banner" );
			}
			else
			{
				BitmapImage B = await Image.NewBitmap();

				Worker.UIInvoke( () =>
				{
					Image.SetSourceFromUrl( B, Path );
					NotifyChanged( "Banner" );
				} );

				_banner = B;
			}
		}

	}
}