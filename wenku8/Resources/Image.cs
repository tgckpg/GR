using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Messaging;

namespace wenku8.Resources
{
    using Model.Book;
    using Settings;
    using Storage;
    using System;

    static class Image
	{
		public static byte[] EMPTY_IMAGE = new byte[]{
			71, 73, 70, 56, 57, 97, 1, 0, 1
			, 0, 128, 0, 0, 255, 255, 255
			, 0, 0, 0, 33, 249, 4, 1, 0, 0
			, 0, 0, 44, 0, 0, 0, 0, 1, 0, 1
			, 0, 0, 2, 2, 68, 1, 0, 59
		};

        public static async Task<BitmapImage> NewBitmap( Uri Uri = null )
        {
            TaskCompletionSource<BitmapImage> bmp = new TaskCompletionSource<BitmapImage>();
            Worker.UIInvoke( () => bmp.SetResult( Uri == null ? new BitmapImage() : new BitmapImage( Uri ) ) );
            return await bmp.Task;
        }

        public static async void SetSourceFromUrl( this BitmapImage Image, string Url )
        {
            if ( string.IsNullOrEmpty( Url ) )
            {
                await Image.SetSourceAsync( new MemoryStream( EMPTY_IMAGE ).AsRandomAccessStream() );
                return;
            }

            try
            {
                await Image.SetSourceAsync( Shared.Storage.GetStream( Url ).AsRandomAccessStream() );
            }
            catch ( Exception ex )
            {
                MessageBus.Send(
                    new Message( typeof( ActionCenter ), ex.Message )
                );
            }
        }

        internal async static Task<string> CreateTileImage( BookItem b )
        {
            string TilePath = FileLinks.ROOT_TILE + b.Id + "_tile.jpg";
            if ( Shared.Storage.FileExists( TilePath ) ) return TilePath;

            BitmapImage bi = new BitmapImage();
            await bi.SetSourceAsync( Shared.Storage.GetStream( b.CoverPath ).AsRandomAccessStream() );

            int Width = bi.PixelWidth;
            int Height = bi.PixelHeight;
            bi = null;

            using ( Stream readStream = Shared.Storage.GetStream( b.CoverPath ) )
            {
                var decoder = await BitmapDecoder.CreateAsync( readStream.AsRandomAccessStream() );

                using ( InMemoryRandomAccessStream writeStream = new InMemoryRandomAccessStream() )
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync( writeStream, decoder );

                    encoder.BitmapTransform.Bounds = new BitmapBounds()
                    {
                        X = 0, Y = 0
                        , Width = 150, Height = 150
                    };

                    double R = 150.0 / ( Width > Height ? Height : Width );
                    encoder.BitmapTransform.ScaledWidth = ( uint ) Math.Round( Width * R, 0 );
                    encoder.BitmapTransform.ScaledHeight = ( uint ) Math.Round( Height * R, 0 );
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                    await encoder.FlushAsync();

                    Shared.Storage.WriteStream( TilePath, writeStream.GetInputStreamAt( 0 ).AsStreamForRead() );
                }
            }

            return TilePath;
        }

        public static async void Destroy( ImageSource Source )
        {
            if ( Source == null ) return;
            if( Source is BitmapImage )
            {
                try
                {
                    using ( Stream s = new MemoryStream( new byte[] { 0 } ) )
                    {
                        await ( Source as BitmapImage ).SetSourceAsync( s.AsRandomAccessStream() );
                    }
                }
                catch( Exception )
                {

                }
            }
        }

        public static async Task<ImageSource> EmptyImage()
        {
            BitmapImage b = await NewBitmap();
            using ( Stream s = new MemoryStream( EMPTY_IMAGE ) )
            {
                await b.SetSourceAsync( s.AsRandomAccessStream() );
            }

            return b;
        }
    }

}
