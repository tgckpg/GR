using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Migrations.Books
{
    public partial class SScript : Migration
    {
		protected override void Up( MigrationBuilder migrationBuilder )
		{
			migrationBuilder.CreateTable(
				name: "SScripts",
				columns: table => new
				{
					Id = table.Column<int>( nullable: false )
						.Annotation( "Sqlite:Autoincrement", true ),
					DateModified = table.Column<DateTime>( nullable: false )
						.Annotation( "AutoNow", ( byte ) 3 ),
					OnlineId = table.Column<Guid>( nullable: true ),
					Title = table.Column<string>( nullable: true ),
					RawData = table.Column<byte[]>( nullable: true ),
					Type = table.Column<string>( nullable: false )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_SScripts", x => x.Id );
				} );

			migrationBuilder.CreateTable(
				name: "M0003_Books",
				columns: table => new
				{
					Id = table.Column<int>( nullable: false )
						.Annotation( "Sqlite:Autoincrement", true ),
					DateModified = table.Column<DateTime>( nullable: false )
						.Annotation( "AutoNow", ( byte ) 3 ),
					Description = table.Column<string>( nullable: true ),
					Fav = table.Column<bool>( nullable: false ),
					Json_Meta = table.Column<string>( nullable: true ),
					LastAccess = table.Column<DateTime>( nullable: true ),
					TextLayout = table.Column<byte>( nullable: false ),
					Title = table.Column<string>( nullable: false ),
					Type = table.Column<byte>( nullable: false ),
					ZItemId = table.Column<string>( nullable: true ),
					ZoneId = table.Column<string>( nullable: false ),
					ScriptId = table.Column<string>( nullable: true )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_Books", x => x.Id );
					table.ForeignKey(
						name: "FK_Books_SScripts_ScriptId",
						column: x => x.ScriptId,
						principalTable: "SScripts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			migrationBuilder.Sql( "PRAGMA foreign_keys = OFF", true );
			migrationBuilder.Sql( "INSERT INTO M0003_Books SELECT *, NULL as ScriptId FROM Books", true );
			migrationBuilder.DropTable( name: "Books" );
			migrationBuilder.RenameTable( name: "M0003_Books", newName: "Books" );
			migrationBuilder.CreateIndex( name: "IX_Books_Title", table: "Books", column: "Title" );
			migrationBuilder.CreateIndex( name: "IX_Books_ZItemId", table: "Books", column: "ZItemId" );
			migrationBuilder.CreateIndex( name: "IX_Books_ZoneId", table: "Books", column: "ZoneId" );
			migrationBuilder.CreateIndex( name: "IX_Books_ScriptId", table: "Books", column: "ScriptId" );

			migrationBuilder.Sql( "PRAGMA foriegn_key_check", true );
			migrationBuilder.Sql( "PRAGMA foriegn_keys = ON" );
		}

		protected override void Down( MigrationBuilder migrationBuilder )
		{
			migrationBuilder.DropForeignKey( name: "FK_Books_SScripts_ScriptId", table: "Books" );
			migrationBuilder.DropTable( name: "SScripts" );
			migrationBuilder.DropIndex( name: "IX_Books_ScriptId", table: "Books" );
			migrationBuilder.DropColumn( name: "ScriptId", table: "Books" );
		}
    }
}
