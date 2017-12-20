using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Migrations.Books
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookInfo",
                columns: table => new
                {
                    BookId = table.Column<string>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    FavCount = table.Column<string>(nullable: true),
                    Intro = table.Column<string>(nullable: true),
                    Length = table.Column<string>(nullable: true),
                    LongDescription = table.Column<string>(nullable: true),
                    Press = table.Column<string>(nullable: true),
                    PushCount = table.Column<string>(nullable: true),
                    RecentUpdate = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StatusLong = table.Column<string>(nullable: true),
                    TodayHitCount = table.Column<string>(nullable: true),
                    TotalHitCount = table.Column<string>(nullable: true),
                    UpdateStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookInfo", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Description = table.Column<string>(nullable: true),
                    InfoBookId = table.Column<string>(nullable: true),
                    TextLayout = table.Column<byte>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_BookInfo_InfoBookId",
                        column: x => x.InfoBookId,
                        principalTable: "BookInfo",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_InfoBookId",
                table: "Books",
                column: "InfoBookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "BookInfo");
        }
    }
}
