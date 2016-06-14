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

namespace wenku8.Model.ListItem
{
    using Resources;

    class ImageThumb : ActiveData
    {
        public static readonly string ID = typeof( ImageThumb ).Name;

        private uint Width = 0;
        private uint Height = 0;

        private string _Location;
        public string Location
        {
            get { return _Location; }
            private set { _Location = value; }
        }

        private ImageSource Img;

        public string Reference;

        public ImageSource ImgSrc
        {
            get
            {
                return Img;
            }
            set
            {
                Img = value;
                NotifyChanged( "ImgSrc", "IsDownloadNeeded" );
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
            if( Width != null ) this.Width = ( uint ) Width;
            if( Height != null ) this.Height = ( uint ) Height;

            if( this.Width == 0 && this.Height == 0 )
            {
                throw new ArgumentNullException( "You must at least set one of the Width / Height" );
            }

            this.Location = Location;
        }

        public async Task<ImageSource> GetFull()
        {
            AsyncTryOut<Stream> Library;

            BitmapImage b = new BitmapImage();

            if( Library = await Shared.Storage.TryGetImage( Location ) )
            {
                await SetBitmap( b, Library.Out.AsRandomAccessStream() );
            }
            else if( Shared.Storage.FileExists( Location ) )
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

            if( Library = await Shared.Storage.TryGetImage( Location ) )
            {
                if( 0 < Width ) b.DecodePixelWidth = ( int ) Width;
                if( 0 < Height ) b.DecodePixelHeight = ( int ) Height;

                await SetBitmap( b, Library.Out.AsRandomAccessStream() );
            }
            else if( Shared.Storage.FileExists( Location ) )
            {
                if( 0 < Width ) b.DecodePixelWidth = ( int ) Width;
                if( 0 < Height ) b.DecodePixelHeight = ( int ) Height;

                await SetBitmap( b, Shared.Storage.GetStream( Location ).AsRandomAccessStream() );
            }
            else
            {
                b.SetSourceFromUrl( null );
            }

            ImgSrc = b;
        }
    }
}
