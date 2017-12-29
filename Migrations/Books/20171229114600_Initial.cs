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
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Description = table.Column<string>(nullable: true),
                    TextLayout = table.Column<byte>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    ZItemId = table.Column<string>(nullable: true),
                    ZoneId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Author = table.Column<string>(nullable: true),
                    BookId = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_BookInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookInfo_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Volumes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Index = table.Column<int>(nullable: false),
                    Json_Meta = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Volumes_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                        .Annotation("AutoNow", (byte)3),
                    Index = table.Column<int>(nullable: false),
                    Json_Meta = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    VolumeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chapters_Volumes_VolumeId",
                        column: x => x.VolumeId,
                        principalTable: "Volumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChapterContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChapterId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterContents_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChapterImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChapterId = table.Column<int>(nullable: false),
                    Json_Urls = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterImages_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_BookInfo_BookId",
                table: "BookInfo",
                column: "BookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_BookId",
                table: "Chapters",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_VolumeId",
                table: "Chapters",
                column: "VolumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterContents_ChapterId",
                table: "ChapterContents",
                column: "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChapterImages_ChapterId",
                table: "ChapterImages",
                column: "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Volumes_BookId",
                table: "Volumes",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookInfo");

            migrationBuilder.DropTable(
                name: "ChapterContents");

            migrationBuilder.DropTable(
                name: "ChapterImages");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Volumes");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
