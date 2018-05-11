using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Migrations.FTSData
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql( "CREATE VIRTUAL TABLE IF NOT EXISTS \"FTSChapters\" USING fts5( \"ChapterId\", \"Text\" )" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable( name: "FTSChapters");
        }
    }
}
