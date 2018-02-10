using GR.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Logging;

namespace GR.Database.Contexts
{
	class BooksContext : DbContext
	{
		public DbSet<Book> Books { get; set; }
		public DbSet<BookInfo> BookInfo { get; set; }
		public DbSet<Volume> Volumes { get; set; }
		public DbSet<Chapter> Chapters { get; set; }
		public DbSet<ChapterImage> ChapterImages { get; set; }
		public DbSet<ChapterContent> ChapterContents { get; set; }

		public DbSet<Anchor> Anrchors { get; set; }

		public ILoggerFactory GRLoggingFacility => new GRLoggerFactory();

		public string FileLocation { get; private set; }

		public BooksContext()
		{
			FileLocation = "books.db";
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

		public Book QueryBook( BookType Type, string ZoneId, string ZItemId )
		{
			lock( this )
			{
				return _QueryBook( Type, ZoneId, ZItemId );
			}
		}

		public void Delete( BookType SType, string ZoneId, string ZItemId )
		{
			lock( this )
			{
				Book Bk = _QueryBook( SType, ZoneId, ZItemId );
				if ( Bk != null )
				{
					Books.Remove( Bk );
					SaveChanges();
				}
			}
		}

		public IEnumerable<Book> QueryBook( Func<Book, bool> QueryExp )
		{
			lock ( this )
			{
				return Books.Include( x => x.Info ).Where( QueryExp );
			}
		}

		public Book GetBook( string ZoneId, string ZItemId, BookType SrcType )
		{
			lock ( this )
			{
				Book Bk = UnsavedBooks.FirstOrDefault( b => b.ZoneId == ZoneId && b.ZItemId == ZItemId && b.Type == SrcType );

				if ( Bk == null )
				{
					Bk = _QueryBook( SrcType, ZoneId, ZItemId );
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

					UnsavedBooks.Add( Bk );
				}

				return Bk;
			}
		}

		public void RemoveUnsaved( Book Bk )
		{
			lock ( this )
				UnsavedBooks.Remove( Bk );
		}

		private Book _QueryBook( BookType Type, string ZoneId, string ZItemId )
		{
			return Books.Include( x => x.Info ).FirstOrDefault( x => x.ZoneId == ZoneId && x.ZItemId == ZItemId && x.Type == Type );
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

		public void SaveBook( Book Bk )
		{
			lock( this )
			{
				_SaveBook( Bk );
				SaveChanges();
			}
		}

		public void SaveBooks( IEnumerable<Book> Items )
		{
			lock ( this )
			{
				foreach( Book Bk in Items )
				{
					_SaveBook( Bk );
				}

				SaveChanges();
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