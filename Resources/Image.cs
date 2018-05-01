using Microsoft.EntityFrameworkCore;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace GR.Resources
{
	using Database.Contexts;
	using Database.Schema;
	using Model.Book;
	using Model.ListItem;
	using Settings;
	using System.Collections.Generic;
	using System.Linq;

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

			Image.UriSource = new Uri( "ms-appdata:///local/" + Url, UriKind.Absolute );
		}

		public static async void SetSourceFromISF( this BitmapImage Image, IStorageFile File )
		{
			if ( File == null )
			{
				await Image.SetSourceAsync( new MemoryStream( EMPTY_IMAGE ).AsRandomAccessStream() );
				return;
			}

			Uri _uri = new Uri( File.Path, UriKind.Absolute );
			if ( _uri.Scheme == "file" )
			{
				IStorageFile LocalFile = await AppStorage.StaticTemp( File );
				Image.UriSource = new Uri( LocalFile.Path, UriKind.Absolute );
			}
			else
			{
				Image.UriSource = new Uri( File.Path, UriKind.Absolute );
			}
		}

		internal async static Task<string> CreateTileImage( BookItem b )
		{
			string TilePath = FileLinks.ROOT_TILE + b.PathId + ".tile";
			if ( Shared.Storage.FileExists( TilePath ) )
				goto ReturnAppStoragePath;

			if ( !b.CoverExist )
			{
				return "ms-appx:///Assets/Samples/Empty150.png";
			}

			using ( Stream s = b.CoverStream() )
			{
				BitmapImage bi = new BitmapImage();
				await bi.SetSourceAsync( s.AsRandomAccessStream() );
				int Width = bi.PixelWidth;
				int Height = bi.PixelHeight;
				bi = null;

				s.Seek( 0, SeekOrigin.Begin );

				var decoder = await BitmapDecoder.CreateAsync( s.AsRandomAccessStream() );
				using ( InMemoryRandomAccessStream writeStream = new InMemoryRandomAccessStream() )
				{
					BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync( writeStream, decoder );

					encoder.BitmapTransform.Bounds = new BitmapBounds() { X = 0, Y = 0, Width = 150, Height = 150 };

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
						if ( Book.CoverExist )
						{
							using ( Stream s = Book.CoverStream() )
								await PunchImage( dev, ds, s, Width, Height, Text );
						}
						else
						{
							using ( Stream s = ( await AppStorage.AppXGetBytes( "/Assets/Samples/Empty150.png" ) ).AsBuffer().AsStream() )
								await PunchImage( dev, ds, s, Width, Height, Text );
						}
					}

					string FileName = FileLinks.ROOT_TILE + Book.PathId + string.Format( ".{0}x{1}.tile", ( int ) Width, ( int ) Height );
					await RenderTarget.SaveAsync( Path.Combine( ApplicationData.Current.LocalFolder.Path, FileName ), CanvasBitmapFileFormat.Png );
					return "ms-appdata:///local/" + FileName;
				}
			}
			catch ( Exception ex )
			{
				Logger.Log( "LiveTile", ex.Message, LogType.ERROR );
			}

			return null;
		}

		private static async Task PunchImage( CanvasDevice dev, CanvasDrawingSession ds, Stream ImageStream, float Width, float Height, string Text )
		{
			CanvasTextFormat Format = new CanvasTextFormat()
			{
				FontSize = Width * 0.169014f
				,
				VerticalAlignment = CanvasVerticalAlignment.Top
				,
				HorizontalAlignment = CanvasHorizontalAlignment.Left
				,
				FontFamily = "Segoe MDL2 Assets"
			};

			CanvasBitmap Bitmap = await CanvasBitmap.LoadAsync( dev, ImageStream.AsRandomAccessStream() );

			CanvasCommandList ScaledBitmap = new CanvasCommandList( ds );
			uint FillSize = Math.Min( Bitmap.SizeInPixels.Width, Bitmap.SizeInPixels.Height );

			using ( CanvasDrawingSession sds = ScaledBitmap.CreateDrawingSession() )
			{
				sds.DrawImage( Bitmap, new Rect( 0, 0, Width, Height ), new Rect( 0, 0, FillSize, FillSize ) );
			}

			CanvasGeometry ImageRect = CanvasGeometry.CreateRectangle( dev, 0, 0, Width, Height );
			CanvasGeometry Badge = CanvasGeometry.CreateCircle( dev, new Vector2( Width - Width * 0.211267f, Height - Height * 0.211267f ), Width * 0.140845f );

			CanvasGeometry Combined = ImageRect.CombineWith( Badge, Matrix3x2.CreateTranslation( 0, 0 ), CanvasGeometryCombine.Exclude );

			CanvasTextLayout TextLayout = new CanvasTextLayout( dev, Text, Format, Width, Height );

			CanvasImageBrush Brush = new CanvasImageBrush( ds, ScaledBitmap );
			Brush.SourceRectangle = new Rect( 0, 0, FillSize, FillSize );

			ds.FillGeometry( Combined, Brush );
			ds.FillGeometry( Badge, Color.FromArgb( 172, 0, 0, 0 ) );

			ds.DrawTextLayout( TextLayout, Width - Width * 0.29577f, Height - Height * 0.29577f, Colors.White );
		}

		public static async void Destroy( ImageSource Source )
		{
			if ( Source == null ) return;
			if ( Source is BitmapImage )
			{
				try
				{
					using ( Stream s = new MemoryStream( new byte[] { 0 } ) )
					{
						await ( Source as BitmapImage ).SetSourceAsync( s.AsRandomAccessStream() );
					}
				}
				catch ( Exception )
				{
					// Intentionally setting invalid source to release the memory
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

		public static async Task CaptureScreen( string SaveLocation, UIElement element, int Width, int Height )
		{
			RenderTargetBitmap b = new RenderTargetBitmap();
			await b.RenderAsync( element, Width, Height );
			IBuffer Pixels = await b.GetPixelsAsync();

			using ( InMemoryRandomAccessStream writeStream = new InMemoryRandomAccessStream() )
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync( BitmapEncoder.PngEncoderId, writeStream );
				encoder.SetPixelData( BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, ( uint ) Width, ( uint ) Height, 96, 96, Pixels.ToArray() );
				await encoder.FlushAsync();

				Shared.Storage.WriteStream( SaveLocation, writeStream.GetInputStreamAt( 0 ).AsStreamForRead() );
			}
		}

		public static async Task WriteXRBK( BookItem Bk, IStorageFile BkFile )
		{
			using ( Stream s = await BkFile.OpenStreamForWriteAsync() )
			{
				if ( Bk.CoverExist )
				{
					BitmapEncoder Enc;
					Enc = await BitmapEncoder.CreateAsync( BitmapEncoder.JpegEncoderId, s.AsRandomAccessStream() );
					using ( Stream ss = Bk.CoverStream() )
					{
						BitmapDecoder Dec = await BitmapDecoder.CreateAsync( ss.AsRandomAccessStream() );
						BitmapFrame ssFrame = await Dec.GetFrameAsync( 0 );
						Enc.SetPixelData(
							ssFrame.BitmapPixelFormat
							, ssFrame.BitmapAlphaMode
							, ssFrame.PixelWidth, ssFrame.PixelHeight
							, ssFrame.DpiX, ssFrame.DpiY
							, ( await ssFrame.GetPixelDataAsync() ).DetachPixelData() );
					}

					await Enc.FlushAsync();
				}

				IStorageFile DataFile = await AppStorage.MkTemp( "bkdata" );

				using ( BooksContext BkContext = new BooksContext( DataFile.Path ) )
				{
					BkContext.Database.Migrate();

					List<Database.Models.Chapter> Chapters = await Shared.BooksDb.SafeRun( x => x.Chapters.Where( c => c.Book == Bk.Entry ).ToListAsync() );
					Chapters.ForEach( x => BkContext.Add( x ) );

					List<Database.Models.ChapterContent> ChapterContents = await Shared.BooksDb.SafeRun( x => x.ChapterContents.Where( c => Chapters.Contains( c.Chapter ) ).ToListAsync() );
					ChapterContents.ForEach( x => BkContext.Add( x ) );

					List<Database.Models.ChapterImage> ChapterImages = await Shared.BooksDb.SafeRun( x => x.ChapterImages.Where( c => Chapters.Contains( c.Chapter ) ).ToListAsync() );
					ChapterImages.ForEach( x => BkContext.Add( x ) );

					List<Database.Models.Volume> Volumes = await Shared.BooksDb.SafeRun( x => x.Volumes.Where( c => c.Book == Bk.Entry ).ToListAsync() );
					Volumes.ForEach( x => BkContext.Add( x ) );

					List<Database.Models.CustomConv> CustomConvs = await Shared.BooksDb.SafeRun( x => x.CustomConvs.Where( c => c.Book == Bk.Entry ).ToListAsync() );
					CustomConvs.ForEach( x => BkContext.Add( x ) );

					if ( Bk is Model.Book.Spider.BookInstruction Inst )
					{
						string SpiderDef = Model.ListItem.SpiderBook.GetSettings( Inst.ZoneId, Inst.ZItemId )?.ToString();

						if( SpiderDef != null )
						{
							Bk.Entry.Meta[ "SpiderDef" ] = new ZData { StringValue = SpiderDef }.GetBase64Raw();
						}
					}

					BkContext.Add( Bk.Entry );
					BkContext.SaveChanges();

					Bk.Entry.Meta.Remove( "SpiderDef" );
				}

				ZData Data = new ZData { BytesValue = await DataFile.ReadAllBytes() };
				await DataFile.DeleteAsync();

				s.Seek( 0, SeekOrigin.End );

				BinaryWriter Writer = new BinaryWriter( s );
				Writer.Write( Data.RawBytes );
				Writer.Write( Data.RawBytes.Length );
				Writer.Write( "XRBK".ToCharArray() );
				Writer.Flush();
			}
		}

		public static async Task ReadXRBK( IStorageFile ISF )
		{
			if ( ISF == null )
				return;

			using ( Stream s = await ISF.OpenStreamForReadAsync() )
			{
				s.Seek( -8, SeekOrigin.End );
				BinaryReader Reader = new BinaryReader( s );

				int DataSize = Reader.ReadInt32();
				string RBOM = new string( Reader.ReadChars( 4 ) );

				if ( RBOM != "XRBK" )
					return;

				s.Seek( -DataSize - 8, SeekOrigin.End );

				ZData Data = new ZData() { RawBytes = new byte[ DataSize ] };
				await s.ReadAsync( Data.RawBytes, 0, DataSize );

				IStorageFile DataFile = await AppStorage.MkTemp( "bkdata" );
				await DataFile.WriteBytes( Data.BytesValue );

				using ( BooksContext BkContext = new BooksContext( DataFile.Path ) )
				{
					Database.Models.Book Entry = BkContext.Books.FirstOrDefault();
					if ( Entry == null )
						return;

					if ( Entry.Meta.TryGetValue( "SpiderDef", out string SpDef ) )
					{
						Data.SetBase64Raw( SpDef );
						SpiderBook SBook = await SpiderBook.CreateSAsync( Entry.ZoneId, Entry.ZItemId, null );
						XRegistry XReg = new XRegistry( Data.StringValue, SBook.MetaLocation );

						System.Diagnostics.Debugger.Break();
					}
				}
			}

		}

	}
}