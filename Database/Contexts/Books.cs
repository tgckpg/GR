using GR.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Database.Contexts
{
	class BooksContext : DbContext
	{
		public DbSet<Book> Books { get; set; }
		public DbSet<BookInfo> BookInfo { get; set; }

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			optionsBuilder.UseSqlite( "Data Source=books.db" );
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, GRMigrationsSqlGenerator>();
			optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, GRMigrationsAnnotationProvider>();
		}

		protected override void OnModelCreating( ModelBuilder modelBuilder )
		{
			EntityTypeBuilder<Book> BookEntity = modelBuilder.Entity<Book>();
			BookEntity.HasIndex( b => b.ZoneId );
			BookEntity.HasIndex( b => b.ZItemId );
			BookEntity.HasIndex( b => b.Title );
		}
	}
}