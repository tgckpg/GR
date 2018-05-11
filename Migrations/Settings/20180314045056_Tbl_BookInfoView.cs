using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Migrations.Settings
{
    public partial class Tbl_BookInfoView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookInfoView",
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
                    table.PrimaryKey("PK_BookInfoView", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookInfoView");
        }
    }
}