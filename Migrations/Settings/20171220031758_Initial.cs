using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Migrations.Settings
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentReader",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentReader", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "System",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Theme",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Theme", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentReader");

            migrationBuilder.DropTable(
                name: "System");

            migrationBuilder.DropTable(
                name: "Theme");
        }
    }
}
