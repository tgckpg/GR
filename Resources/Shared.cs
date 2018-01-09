using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

namespace GR.Resources
{
	using CompositeElement;
	using Database.Contexts;
	using Database.Models;
	using Model;
	using Model.Book;
	using Model.REST;
	using Storage;

	static class Shared
	{
		public static GeneralStorage Storage;

		// Books Cache used by loaders
		public static BookPool BooksCache = new BookPool( 5 );
		public static BookItem CurrentBook;

		// The default settings by locale
		public static LocaleDefaults LocaleDefaults = new LocaleDefaults();

		public static SharersRequest ShRequest;

		private static StringResources LoadMesgRes;

		public static TradChinese TC;

		private static BooksContext _Books;
		public static BooksContext BooksDb
		{
			get
			{
				if ( _Books == null )
				{
					_Books = new BooksContext();
				}
				return _Books;
			}
		}

		public static Book QueryBook( string id )
		{
			Book Bk = BooksDb.Books.Find( id );
			if ( Bk != null )
			{
				BooksDb.Entry( Bk ).Reference( b => b.Info ).Load();
			}
			return Bk;
		}

		public static List<Book> UnsavedBooks = new List<Book>();

		public static Book GetBook( string ZoneId, string ZItemId, BookType SrcType )
		{
			lock ( UnsavedBooks )
			{
				Book Bk = UnsavedBooks.FirstOrDefault( b => b.ZoneId == ZoneId && b.ZItemId == ZItemId && b.Type == SrcType );

				if ( Bk == null )
				{
					Bk = BooksDb.Books.FirstOrDefault( b => b.ZoneId == ZoneId && b.ZItemId == ZItemId && b.Type == SrcType );
				}

				if ( Bk == null )
				{
					Bk = new Book()
					{
						Type = SrcType,
						ZoneId = ZoneId,
						ZItemId = ZItemId,
						Title = "[Unknown]",
						Info = new Database.Models.BookInfo()
					};

					UnsavedBooks.Add( Bk );
				}
				else
				{
					BooksDb.Entry( Bk ).Reference( b => b.Info ).Load();
				}

				return Bk;
			}
		}

		public static void SaveBook( Book Bk )
		{
			lock( UnsavedBooks )
			{
				if ( UnsavedBooks.Contains( Bk ) )
				{
					BooksDb.Books.Add( Bk );
					UnsavedBooks.Remove( Bk );
				}
				else
				{
					BooksDb.Books.Update( Bk );
				}

				BooksDb.SaveChanges();
			}
		}

		public static IEnumerable<Book> QueryBooks( BookType SrcType )
		{
			return BooksDb.Books.Where( b => b.Type == SrcType );
		}

		public static void LoadMessage( string MESG_ID, params string[] args )
		{
			Worker.UIInvoke( () =>
			{
				if ( LoadMesgRes == null ) LoadMesgRes = new StringResources( "LoadingMessage" );
				string mesg = LoadMesgRes.Str( MESG_ID );

				mesg = string.IsNullOrEmpty( mesg ) ? MESG_ID : mesg;

				if ( 0 < args.Length )
				{
					mesg = string.Format( mesg, args );
				}

				MessageBus.SendUI( typeof( LoadingMask ), mesg );
			} );
		}
	}
}