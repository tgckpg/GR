using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.Storage;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.IO;
using Net.Astropenguin.Loaders;

namespace GR.Model.Pages
{
	using Book;
	using Book.Spider;
	using CompositeElement;
	using Database.Contexts;
	using Database.Models;
	using Database.Schema;
	using Ext;
	using GSystem;
	using ListItem;
	using Resources;
	using Settings;

	sealed class ItemProcessor
	{
		public static async Task ProcessLocal( LocalBook LB )
		{
			await LB.Process();
			if ( LB is SpiderBook SB )
			{
				BookInstruction Inst = SB.GetBook();
				if ( Inst.Packable )
				{
					Inst.PackVolumes( SB.GetPPConvoy() );
				}
			}
		}

		public static async Task<BookItem> GetBookFromId( string Id )
		{
			bool IsBookSpider = false;
			IsBookSpider = Guid.TryParse( Id, out Guid _Guid );

			if ( IsBookSpider )
			{
				SpiderBook Book = await SpiderBook.CreateSAsync( Id );
				if ( Book.ProcessSuccess ) return Book.GetBook();
			}
			else if ( int.TryParse( Id, out int _Id ) )
			{
				if ( X.Exists ) return GetBookEx( Id );

				LocalTextDocument Doc = new LocalTextDocument( Id );
				if ( Doc.IsValid ) return Doc;
			}

			return null;
		}

		public static BookItem GetBookItem( Book Bk )
		{
			if ( X.Exists && Bk.Type.HasFlag( BookType.W ) )
			{
				return X.Call<BookItem>( XProto.BookItemEx, "Create", Bk );
			}

			switch ( Bk.Type )
			{
				case BookType.S:
					return new BookInstruction( Bk );
				case BookType.L:
					return new LocalTextDocument( Bk );
			}

			return null;
		}

		public static Task<BookItem> GetBookFromTileCmd( string Command )
		{
			string[] Cmd = Command.Split( '|' );

			if ( Cmd.Length == 2 && !string.IsNullOrEmpty( Cmd[ 1 ] ) )
			{
				return GetBookFromId( Cmd[ 1 ] );
			}
			else if ( Cmd.Length == 3 )
			{
				return Task.Run( () =>
				{
					BookType BType = ( BookType ) Enum.Parse( typeof( BookType ), Cmd[ 0 ] );
					Book Bk = Shared.BooksDb.QueryBook( BType, Cmd[ 1 ], Cmd[ 2 ] );

					if ( Bk == null )
						return null;

					return GetBookItem( Bk );
				} );
			}

			return Task.FromResult<BookItem>( null );
		}

		public static BookItem GetBookEx( string Id )
		{
			BookItem B = X.Call<BookItem>( XProto.BookItemEx, "GetItem", Id );
			B.XSetProp( "Mode", X.Const<string>( XProto.WProtocols, "ACTION_BOOK_META" ) );

			return B;
		}

		public static async Task WriteXRBK( BookItem Bk, IStorageFile BkFile )
		{
			using ( Stream s = await BkFile.OpenStreamForWriteAsync() )
			{
				await Image.WriteCoverStream( Bk, s );

				IStorageFile DataFile = await AppStorage.MkTemp( "bkdata" );

				using ( BooksContext BkContext = new BooksContext( DataFile.Path ) )
				{
					BkContext.Database.Migrate();

					List<Chapter> Chapters = await Shared.BooksDb.SafeRun( x => x.Chapters.Where( c => c.Book == Bk.Entry ).ToListAsync() );
					Chapters.ForEach( x => BkContext.Add( x ) );

					List<ChapterContent> ChapterContents = await Shared.BooksDb.SafeRun( x => x.ChapterContents.Where( c => Chapters.Contains( c.Chapter ) ).ToListAsync() );
					ChapterContents.ForEach( x => BkContext.Add( x ) );

					List<ChapterImage> ChapterImages = await Shared.BooksDb.SafeRun( x => x.ChapterImages.Where( c => Chapters.Contains( c.Chapter ) ).ToListAsync() );
					ChapterImages.ForEach( x => BkContext.Add( x ) );

					List<Volume> Volumes = await Shared.BooksDb.SafeRun( x => x.Volumes.Where( c => c.Book == Bk.Entry ).ToListAsync() );
					Volumes.ForEach( x => BkContext.Add( x ) );

					List<CustomConv> CustomConvs = await Shared.BooksDb.SafeRun( x => x.CustomConvs.Where( c => c.Book == Bk.Entry ).ToListAsync() );
					CustomConvs.ForEach( x => BkContext.Add( x ) );

					if ( Bk is BookInstruction Inst )
					{
						string SpiderDef = SpiderBook.GetSettings( Inst.Entry )?.ToString();

						if ( SpiderDef != null )
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

		public static bool RequestOpenXRBK( object Args, out IStorageFile ISF )
		{
			ISF = Args as IStorageFile;

			if ( ISF == null && Args is FileActivatedEventArgs FileArgs )
			{
				ISF = FileArgs.Files.First() as IStorageFile;
			}

			return ISF != null;
		}

		public static async Task<BookItem> OpenXRBK( IStorageFile ISF )
		{
			using ( Stream s = await ISF.OpenStreamForReadAsync() )
			{
				s.Seek( -8, SeekOrigin.End );
				BinaryReader Reader = new BinaryReader( s );

				int DataSize = Reader.ReadInt32();
				string RBOM = new string( Reader.ReadChars( 4 ) );

				if ( RBOM != "XRBK" )
					return null;

				s.Seek( -DataSize - 8, SeekOrigin.End );

				ZData Data = new ZData() { RawBytes = new byte[ DataSize ] };
				await s.ReadAsync( Data.RawBytes, 0, DataSize );

				string DataId = Utils.Md5( Data.RawBytes.AsBuffer() );

				IStorageFile DataFile = await AppStorage.MkTemp( "bkdata" );
				try
				{
					await DataFile.WriteBytes( Data.BytesValue );

					using ( BooksContext RemoteContext = new BooksContext( DataFile.Path ) )
					{
						RemoteContext.Database.Migrate();
						Book RemoteEntry = RemoteContext.Books.Include( x => x.Info ).FirstOrDefault();

						if ( RemoteEntry == null )
							return null;

						// Type chould be hybrid, so we gotta query the book first
						Book LocalEntry =
							Shared.BooksDb.QueryBook( BookType.S, RemoteEntry.ZoneId, RemoteEntry.ZItemId )
							?? Shared.BooksDb.QueryBook( BookType.W, RemoteEntry.ZoneId, RemoteEntry.ZItemId )
							?? Shared.BooksDb.QueryBook( BookType.L, RemoteEntry.ZoneId, RemoteEntry.ZItemId )
							// This one creates the entry
							?? Shared.BooksDb.GetBook( RemoteEntry.ZoneId, RemoteEntry.ZItemId, RemoteEntry.Type )
							;

						await _ImportEntry( DataId, RemoteContext, RemoteEntry, LocalEntry );

						return GetBookItem( LocalEntry );
					}
				}
				finally
				{
					await DataFile.DeleteAsync();
				}
			}
		}

		private static async Task _ImportEntry( string DataId, BooksContext RemoteContext, Book RemoteEntry, Book LocalEntry )
		{
			// Check to see if a local copy already exsits
			if ( Shared.BooksDb.SafeRun( x => x.Entry( LocalEntry ).State != EntityState.Detached ) )
			{
				// Content of this book has already been imported
				if ( LocalEntry.Meta.TryGetValue( "DataId", out string LocalDataId ) && DataId == LocalDataId )
				{
					return;
				}
				else
				{
					bool ConfirmReplace = false;
					bool NeverReplace = false;

					await Worker.RunUITaskAsync( () =>
					{
						StringResources stx = StringResources.Load( "Message" );
						MessageDialog Dialog = UIAliases.CreateDialog(
							stx.Str( "ConfirmReplace" )
							, () => ConfirmReplace = true
							, stx.Str( "Yes" ), stx.Str( "No" )
						);

						Dialog.Commands.Add( new UICommand( stx.Str( "NoAndNever" ), x => NeverReplace = true ) );

						return Popups.ShowDialog( Dialog );
					} );

					if ( !ConfirmReplace )
					{
						if ( NeverReplace )
						{
							LocalEntry.Meta[ "DataId" ] = DataId;
							Shared.BooksDb.SaveBook( LocalEntry );
						}

						return;
					}
				}
			}

			List<Volume> AllVols = await RemoteContext.Volumes.OrderBy( x => x.Index ).ToListAsync();
			foreach ( Volume Vol in AllVols )
			{
				await RemoteContext.Entry( Vol ).Collection( x => x.Chapters ).Query().OrderBy( x => x.Index ).LoadAsync();
			}

			List<Chapter> AllChapters = await RemoteContext.Chapters.Include( x => x.Content ).Include( x => x.Image ).ToListAsync();
			List<CustomConv> AllConvs = await RemoteContext.CustomConvs.OrderBy( x => x.Phase ).ToListAsync();
			List<ChapterContent> AllChContents = await RemoteContext.ChapterContents.ToListAsync();
			List<ChapterImage> AllChImages = await RemoteContext.ChapterImages.ToListAsync();

			AllVols.ForEach( x => { x.Id = 0; x.Book = LocalEntry; } );
			AllChapters.ForEach( x => { x.Id = 0; x.Book = LocalEntry; } );
			AllConvs.ForEach( x => { x.Id = 0; x.Book = LocalEntry; } );
			AllChContents.ForEach( x => x.Id = 0 );
			AllChImages.ForEach( x => x.Id = 0 );

			LocalEntry.Volumes = AllVols;
			LocalEntry.ConvPhases = AllConvs;

			LocalEntry.Title = RemoteEntry.Title;
			LocalEntry.Description = RemoteEntry.Description;

			BookInfo Info = RemoteEntry.Info;
			Info.Id = 0;
			Info.Book = LocalEntry;

			LocalEntry.Info = Info;
			LocalEntry.Meta[ "DataId" ] = DataId;

			if ( LocalEntry.Type.HasFlag( BookType.S ) && RemoteEntry.Meta.TryGetValue( "SpiderDef", out string SpDef ) )
			{
				LocalEntry.Script = new SScript() { Type = AppKeys.SS_BS };
				LocalEntry.Script.Data.SetBase64Raw( SpDef );
			}

			Shared.BooksDb.SaveBook( LocalEntry );
		}

	}
}