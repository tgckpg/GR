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
                    CoverSrcUrl = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    FavCount = table.Column<string>(nullable: true),
                    Json_Flags = table.Column<string>(nullable: true),
                    Json_Others = table.Column<string>(nullable: true),
                    LatestSection = table.Column<string>(nullable: true),
                    Length = table.Column<string>(nullable: true),
                    LongDescription = table.Column<string>(nullable: true),
                    OriginalUrl = table.Column<string>(nullable: true),
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
                    Type = table.Column<byte>(nullable: false),
                    ZItemId = table.Column<string>(nullable: true),
                    ZoneId = table.Column<string>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Volume",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BookId = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Index = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Volume_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chapter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Index = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    VolumeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapter_Volume_VolumeId",
                        column: x => x.VolumeId,
                        principalTable: "Volume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_InfoBookId",
                table: "Books",
                column: "InfoBookId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ZItemId",
                table: "Books",
                column: "ZItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ZoneId",
                table: "Books",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_VolumeId",
                table: "Chapter",
                column: "VolumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Volume_BookId",
                table: "Volume",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chapter");

            migrationBuilder.DropTable(
                name: "Volume");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "BookInfo");
        }
    }
}
