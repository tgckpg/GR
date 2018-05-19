using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace GR.Model.ListItem
{
	using Resources;

	sealed class ImageThumb : ActiveData
	{
		public static readonly string ID = typeof( ImageThumb ).Name;

		public uint Width { get; private set; }
		public uint Height { get; private set; }

		public double FullWidth { get; private set; }
		public double FullHeight { get; private set; }

		public string Location { get; private set; }

		public string Reference;

		private ImageSource _ImgSrc;
		public ImageSource ImgSrc
		{
			get => _ImgSrc;
			set
			{
				_ImgSrc = value;
				NotifyChanged( "ImgSrc", "IsDownloadNeeded", "FullSizeImage" );
			}
		}

		public ImageSource FullSizeImage
		{
			get
			{
				if ( ImgSrc == null )
					return null;

				BitmapImage Bmp = new BitmapImage();
				Bmp.SetSourceFromUrl( Location );
				return Bmp;
			}
		}

		private bool FileAvailable = false;
		public bool IsDownloadNeeded
		{
			get { return !FileAvailable; }
		}

		public Guid Id { get; internal set; }

		public ImageThumb( string Location, uint? Width, uint? Height )
		{
			if ( Width != null ) this.Width = ( uint ) Width;
			if ( Height != null ) this.Height = ( uint ) Height;

			if ( this.Width == 0 && this.Height == 0 )
			{
				throw new ArgumentNullException( "You must at least set one of the Width / Height" );
			}

			this.Location = Location;
		}

		public async Task<ImageSource> GetFull()
		{
			AsyncTryOut<Stream> Library;

			BitmapImage b = new BitmapImage();

			if ( Library = await Shared.Storage.TryGetImage( Location ) )
			{
				await SetBitmap( b, Library.Out.AsRandomAccessStream() );
			}
			else if ( Shared.Storage.FileExists( Location ) )
			{
				await SetBitmap( b, Shared.Storage.GetStream( Location ).AsRandomAccessStream() );
			}
			else
			{
				b.SetSourceFromUrl( null );
			}

			return b;
		}

		private async Task SetBitmap( BitmapImage b, IRandomAccessStream randomAccessStream )
		{
			try
			{
				await b.SetSourceAsync( randomAccessStream );
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, string.Format( "Problematic File {0}: {1}", Location, ex.Message ), LogType.ERROR );
			}

			FileAvailable = true;
		}

		public async Task Set()
		{
			AsyncTryOut<Stream> Library;

			BitmapImage b = new BitmapImage();

			if ( Library = await Shared.Storage.TryGetImage( Location ) )
			{
				await SetImgSrc( b, Library.Out );
			}
			else if ( Shared.Storage.FileExists( Location ) )
			{
				FileAvailable = await SetImgSrc( b, Shared.Storage.GetStream( Location ) );
			}
			else
			{
				b.SetSourceFromUrl( null );
			}

			ImgSrc = b;
		}

		private async Task<bool> SetImgSrc( BitmapImage b, Stream s )
		{
			using ( s )
			{
				try
				{
					(FullWidth, FullHeight) = await Image.GetImageSize( s );
					s.Seek( 0, SeekOrigin.Begin );

					if ( 0 < Width ) b.DecodePixelWidth = ( int ) Width;
					if ( 0 < Height ) b.DecodePixelHeight = ( int ) Height;
					await SetBitmap( b, s.AsRandomAccessStream() );
					return true;
				}
				catch ( Exception )
				{
					return false;
				}
			}
		}
	}
}