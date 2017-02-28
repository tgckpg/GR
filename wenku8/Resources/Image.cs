using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;

using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.IO;
using Net.Astropenguin.Messaging;

namespace wenku8.Resources
{
    using Model.Book;
    using Settings;
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
                MessageBus.Send( typeof( ActionCenter ), ex.Message );
            }
        }

        public static async void SetSourceFromISF( this BitmapImage Image, IStorageFile File )
        {
            if ( File == null )
            {
                await Image.SetSourceAsync( new MemoryStream( EMPTY_IMAGE ).AsRandomAccessStream() );
                return;
            }

            try
            {
                using ( Stream s = await File.OpenStreamForReadAsync() )
                {
                    await Image.SetSourceAsync( s.AsRandomAccessStream() );
                }
            }
            catch ( Exception ex )
            {
                MessageBus.Send( typeof( ActionCenter ), ex.Message );
            }
        }

        internal async static Task<string> CreateTileImage( BookItem b )
        {
            string TilePath = FileLinks.ROOT_TILE + b.Id + ".tile";
            if ( Shared.Storage.FileExists( TilePath ) )
                goto ReturnAppStoragePath;

            if ( !Shared.Storage.FileExists( b.CoverPath ) )
            {
                return "ms-appx:///Assets/Samples/Empty150.png";
            }

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

            ReturnAppStoragePath:
            return "ms-appdata:///local/" + TilePath;
        }

        public static IDisposable CreateCanvasDevice() { return new CanvasDevice(); }

        public static async Task<string> LiveTileBadgeImage( IDisposable CDevice, BookItem Book, float Width, float Height, string Text )
        {
            if ( !( CDevice is CanvasDevice ) ) throw new ArgumentException( "Not a CanvasDevice" );
            CanvasDevice dev = ( CanvasDevice ) CDevice;

            try
            {
                using ( CanvasRenderTarget RenderTarget = new CanvasRenderTarget( dev, Width, Height, 96 ) )
                {
                    using ( CanvasDrawingSession ds = RenderTarget.CreateDrawingSession() )
                    {
                        if ( Shared.Storage.FileExists( Book.CoverPath ) )
                        {
                            using ( Stream s = Shared.Storage.GetStream( Book.CoverPath ) )
                                await PunchImage( dev, ds, s, Width, Height, Text );
                        }
                        else
                        {
                            using ( Stream s = ( await AppStorage.AppXGetBytes( "/Assets/Samples/Empty150.png" ) ).AsBuffer().AsStream() )
                                await PunchImage( dev, ds, s, Width, Height, Text );
                        }
                    }

                    string FileName = FileLinks.ROOT_TILE + Book.Id + string.Format( ".{0}x{1}.tile", ( int ) Width, ( int ) Height );
                    await RenderTarget.SaveAsync( Path.Combine( ApplicationData.Current.LocalFolder.Path, FileName ), CanvasBitmapFileFormat.Png );
                    return "ms-appdata:///local/" + FileName;
                }
            }
            catch ( Exception ex )
            {
                global::System.Diagnostics.Debugger.Break();
            }

            return null;
        }

        private static async Task PunchImage( CanvasDevice dev, CanvasDrawingSession ds, Stream ImageStream, float Width, float Height, string Text )
        {
            CanvasTextFormat Format = new CanvasTextFormat()
            {
                FontSize = 12
                , VerticalAlignment = CanvasVerticalAlignment.Top
                , HorizontalAlignment = CanvasHorizontalAlignment.Left
                , FontFamily = "Segoe MDL2 Assets"
            };

            CanvasBitmap Bitmap = await CanvasBitmap.LoadAsync( dev, ImageStream.AsRandomAccessStream() );

            CanvasCommandList ScaledBitmap = new CanvasCommandList( ds );
            uint FillSize = Math.Min( Bitmap.SizeInPixels.Width, Bitmap.SizeInPixels.Height );

            using ( CanvasDrawingSession sds = ScaledBitmap.CreateDrawingSession() )
            {
                sds.DrawImage( Bitmap, new Rect( 0, 0, Width, Height ), new Rect( 0, 0, FillSize, FillSize ) );
            }

            CanvasGeometry ImageRect = CanvasGeometry.CreateRectangle( dev, 0, 0, Width, Height );
            CanvasGeometry Badge = CanvasGeometry.CreateCircle( dev, new Vector2( Width - 15, Height - 15 ), 10 );

            CanvasGeometry Combined = ImageRect.CombineWith( Badge, Matrix3x2.CreateTranslation( 0, 0 ), CanvasGeometryCombine.Exclude );

            CanvasTextLayout TextLayout = new CanvasTextLayout( dev, Text, Format, Width, Height );

            CanvasImageBrush Brush = new CanvasImageBrush( ds, ScaledBitmap );
            Brush.SourceRectangle = new Rect( 0, 0, FillSize, FillSize );

            ds.FillGeometry( Combined, Brush );

            ds.DrawTextLayout( TextLayout, Width - 21f, Height - 21f, Colors.White );
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