using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Logging;

namespace GR.Database.Contexts
{
	using Models;
	using Model.Interfaces;
	using Settings;

	class BooksContext : DbContext, ISafeContext
	{
		public DbSet<Book> Books { get; set; }
		public DbSet<BookInfo> BookInfo { get; set; }
		public DbSet<Volume> Volumes { get; set; }
		public DbSet<Chapter> Chapters { get; set; }
		public DbSet<ChapterImage> ChapterImages { get; set; }
		public DbSet<ChapterContent> ChapterContents { get; set; }
		public DbSet<CustomConv> CustomConvs { get; set; }

		public ILoggerFactory GRLoggingFacility => new GRLoggerFactory();

		public string FileLocation { get; private set; }

		private object TransactionLock = new Object();

		public BooksContext()
		{
			FileLocation = FileLinks.DB_BOOKS;
		}

		public BooksContext( string FileName )
		{
			FileLocation = FileName;
		}

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			optionsBuilder.UseSqlite( "Data Source=" + FileLocation );
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, GRMigrationsSqlGenerator>();
			optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, GRMigrationsAnnotationProvider>();
			// optionsBuilder.UseLoggerFactory( GRLoggingFacility );
		}

		protected override void OnModelCreating( ModelBuilder modelBuilder )
		{
			EntityTypeBuilder<Book> BookEntity = modelBuilder.Entity<Book>();
			BookEntity.HasIndex( b => b.ZoneId );
			BookEntity.HasIndex( b => b.ZItemId );
			BookEntity.HasIndex( b => b.Title );
		}

		private List<Book> UnsavedBooks = new List<Book>();

		public void Delete( BookType SType, string ZoneId, string ZItemId )
		{
			Book Bk = QueryBook( SType, ZoneId, ZItemId );
			if ( Bk != null )
			{
				Books.Remove( Bk );
				SaveChanges();
			}
		}

		public void Delete( Book Entry )
		{
			Books.Remove( Entry );
			SaveChanges();
		}

		public bool SafeEntry( Book Entry )
		{
			lock( UnsavedBooks )
			{
				return Entry.Id != 0 || UnsavedBooks.Contains( Entry );
			}
		}

		public Book QueryBook( int Id )
		{
			lock ( TransactionLock )
			{
				return Books.Find( Id );
			}
		}

		public void SafeRun( Action<BooksContext> Operation )
		{
			lock ( TransactionLock )
			{
				Operation( this );
			}
		}

		public T SafeRun<T>( Func<T> Operation )
		{
			lock( TransactionLock )
			{
				return Operation();
			}
		}

		public T SafeRun<T>( Func<BooksContext, T> Operation )
		{
			lock ( TransactionLock )
			{
				return Operation( this );
			}
		}

		public IEnumerable<Book> QueryBook( Func<Book, bool> QueryExp )
		{
			lock ( TransactionLock )
			{
				return Books.Include( x => x.Info ).Where( QueryExp );
			}
		}

		public ChapterImage[] GetBookImages( int BookId )
		{
			lock ( TransactionLock )
			{
				return ChapterImages.Where( c => !string.IsNullOrEmpty( c.Json_Urls ) && Chapters.Where( x => x.BookId == BookId ).Select( x => x.Id ).Contains( c.ChapterId ) ).ToArray();
			}
		}

		public Book GetBook( string ZoneId, string ZItemId, BookType SrcType )
		{
			Book Bk;
			lock ( UnsavedBooks )
			{
				Bk = UnsavedBooks.FirstOrDefault( b => b.ZoneId == ZoneId && b.ZItemId == ZItemId && b.Type == SrcType );
			}

			if ( Bk == null )
			{
				Bk = QueryBook( SrcType, ZoneId, ZItemId );
			}

			if ( Bk == null )
			{
				Bk = new Book()
				{
					Type = SrcType,
					ZoneId = ZoneId,
					ZItemId = ZItemId,
					Title = "[Unknown]",
					Info = new BookInfo()
				};

				lock ( UnsavedBooks )
				{
					UnsavedBooks.Add( Bk );
				}
			}

			return Bk;
		}

		public void RemoveUnsaved( Book Bk )
		{
			lock ( UnsavedBooks )
			{
				UnsavedBooks.Remove( Bk );
			}
		}

		public Book QueryBook( BookType Type, string ZoneId, string ZItemId )
		{
			lock ( TransactionLock )
			{
				return Books.Include( x => x.Info ).FirstOrDefault( x => x.ZoneId == ZoneId && x.ZItemId == ZItemId && 0 < ( x.Type & Type ) );
			}
		}

		public void SaveBook( Book Bk )
		{
			lock ( TransactionLock )
			{
				_SaveBook( Bk );
				base.SaveChanges();
			}
		}

		public void SaveBooks( IEnumerable<Book> Items )
		{
			lock ( TransactionLock )
			{
				foreach ( Book Bk in Items )
				{
					_SaveBook( Bk );
				}

				base.SaveChanges();
			}
		}

		private void _SaveBook( Book Bk )
		{
			if ( UnsavedBooks.Contains( Bk ) )
			{
				Books.Add( Bk );
				UnsavedBooks.Remove( Bk );
			}
			else
			{
				Books.Update( Bk );
			}
		}

		public void LoadRef<T, TProperty>( T obj, Expression<Func<T, TProperty>> Path )
			where T: class
			where TProperty : class
		{
			lock ( TransactionLock )
			{
				Entry( obj ).Reference( Path ).Load();
			}
		}

		public Task<List<TProperty>> LoadCollectionAsync<T, TProperty, TOrder>( T obj, Expression<Func<T, IEnumerable<TProperty>>> Path, Expression<Func<TProperty, TOrder>> OrderPath )
			where T: class
			where TProperty : class
		{
			lock ( TransactionLock )
			{
				return Entry( obj ).Collection( Path ).Query().OrderBy( OrderPath ).ToListAsync();
			}
		}

		public List<TProperty> LoadCollection<T, TProperty, TOrder>( T obj, Expression<Func<T, IEnumerable<TProperty>>> Path, Expression<Func<TProperty, TOrder>> OrderPath )
			where T: class
			where TProperty : class
		{
			lock ( TransactionLock )
			{
				return Entry( obj ).Collection( Path ).Query().OrderBy( OrderPath ).ToList();
			}
		}

		public override int SaveChanges()
		{
			lock ( TransactionLock )
			{
				return base.SaveChanges();
			}
		}
	}

	class GRLogScope : IDisposable
	{
		GRLogger Logger;
		public GRLogScope( GRLogger Logger )
		{
			this.Logger = Logger;
			Logger.Indent = true;
		}

		public void Dispose()
		{
			Logger.Indent = false;
		}
	}

	class GRLogger : ILogger
	{
		public string Id { get; internal set; }
		public bool Indent;

		public IDisposable BeginScope<TState>( TState state )
		{
			return new GRLogScope( this );
		}

		public bool IsEnabled( LogLevel logLevel )
		{
			return true;
		}

		public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter )
		{
			Logger.Log( Id, ( Indent ? "\t" : "" ) + formatter( state, exception ) );
		}
	}

	class GRLoggerFactory : ILoggerFactory
	{
		public void AddProvider( ILoggerProvider provider )
		{
			throw new NotSupportedException();
		}

		public ILogger CreateLogger( string categoryName ) => new GRLogger() { Id = categoryName };

		public void Dispose()
		{
			throw new NotSupportedException();
		}
	}

}