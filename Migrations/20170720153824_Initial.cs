using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace libwenku8.Migrations
{
    public partial class Initial : Migration
    {
		protected override void Up( MigrationBuilder migrationBuilder )
		{
			migrationBuilder.CreateTable(
				name: "ContentReader",
				columns: table => new
				{
					Key = table.Column<string>( nullable: false ),
					DateModified = table.Column<DateTime>( nullable: false ),
					Type = table.Column<int>( nullable: false ),
					Value = table.Column<string>( nullable: true )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_ContentReader", x => x.Key );
				} );

			migrationBuilder.CreateTable(
				name: "Theme",
				columns: table => new
				{
					Key = table.Column<string>( nullable: false ),
					DateModified = table.Column<DateTime>( nullable: false ),
					Type = table.Column<int>( nullable: false ),
					Value = table.Column<string>( nullable: true )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_Theme", x => x.Key );
				} );

			migrationBuilder.Sql( "CREATE TRIGGER autodm_theme_update AFTER UPDATE ON Theme FOR EACH ROW BEGIN UPDATE Theme SET DateModified = DATETIME( 'now' ) WHERE Key = old.Key; END" );
			migrationBuilder.Sql( "CREATE TRIGGER autodm_theme_insert AFTER INSERT ON Theme FOR EACH ROW BEGIN UPDATE Theme SET DateModified = DATETIME( 'now' ) WHERE Key = new.Key; END" );
			migrationBuilder.Sql( "CREATE TRIGGER autodm_contentreader_update AFTER UPDATE ON ContentReader FOR EACH ROW BEGIN UPDATE ContentReader SET DateModified = DATETIME( 'now' ) WHERE Key = old.Key; END" );
			migrationBuilder.Sql( "CREATE TRIGGER autodm_contentreader_insert AFTER INSERT ON ContentReader FOR EACH ROW BEGIN UPDATE ContentReader SET DateModified = DATETIME( 'now' ) WHERE Key = new.Key; END" );
		}

		protected override void Down( MigrationBuilder migrationBuilder )
		{
			migrationBuilder.DropTable( name: "ContentReader" );
			migrationBuilder.DropTable( name: "Theme" );
			migrationBuilder.Sql( "DROP TRIGGER autodm_theme_update" );
			migrationBuilder.Sql( "DROP TRIGGER autodm_theme_insert" );
			migrationBuilder.Sql( "DROP TRIGGER autodm_contentreader_update" );
			migrationBuilder.Sql( "DROP TRIGGER autodm_contentreader_insert" );
		}
	}
}
