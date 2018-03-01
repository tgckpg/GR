using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace GR.Database.Contexts
{
	using Models;
	using Settings;

	class FTSDataContext : DbContext
	{
		public DbSet<FTSChapter> FTSChapters { get; set; }

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			optionsBuilder.UseSqlite( "Data Source=" + FileLinks.DB_FTS_DATA );
		}

		public IQueryable<FTSChapter> Search( string Query )
		{
			return FTSChapters.FromSql(
				"SELECT ChapterId, snippet( \"FTSChapters\", 1, '__', '__', '', 8 ) AS \"Text\""
				+ "FROM \"FTSChapters\" WHERE \"FTSChapters\" MATCH {0}"
				, Query
			);
		}
	}
}