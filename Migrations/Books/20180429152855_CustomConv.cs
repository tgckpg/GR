using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GR.Migrations.Books
{
    public partial class CustomConv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable( name: "Anrchors");

            migrationBuilder.CreateTable(
                name: "CustomConvs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(nullable: false),
                    Phase = table.Column<int>(nullable: false),
                    Table = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomConvs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomConvs_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomConvs_BookId",
                table: "CustomConvs",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable( name: "CustomConvs");

            migrationBuilder.CreateTable(
                name: "Anrchors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Json_Meta = table.Column<string>(nullable: true),
                    Ref0 = table.Column<string>(nullable: true),
                    Ref1 = table.Column<string>(nullable: true),
                    Ref2 = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anrchors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anrchors_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anrchors_BookId",
                table: "Anrchors",
                column: "BookId");
        }
    }
}
