﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Database.Contexts
{
	using Models;
	using Settings;

	class SettingsContext: DbContext
	{
		public DbSet<ContentReader> ContentReader { get; set; }
		public DbSet<Theme> Theme { get; set; }
		public DbSet<BookInfoView> BookInfoView { get; set; }
		public DbSet<GRSystem> System { get; set; }
		public DbSet<GRTableConfig> TableConfigs { get; set; }
		public DbSet<GRWidgetConfig> WidgetConfigs { get; set; }

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			optionsBuilder.UseSqlite( "Data Source=" + FileLinks.DB_SETTINGS );
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, GRMigrationsSqlGenerator>();
			optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, GRMigrationsAnnotationProvider>();
		}
	}
}