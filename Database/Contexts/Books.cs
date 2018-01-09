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

		public ILoggerFactory GRLoggingFacility => new GRLoggerFactory();

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			optionsBuilder.UseSqlite( "Data Source=books.db" );
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