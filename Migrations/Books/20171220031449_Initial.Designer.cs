using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GR.Database.Contexts;
using GR.Database.Models;

namespace GR.Migrations.Books
{
    [DbContext(typeof(BooksContext))]
    [Migration("20171220031449_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

            modelBuilder.Entity("GR.Database.Models.Book", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Description");

                    b.Property<string>("InfoBookId");

                    b.Property<byte>("TextLayout");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<byte>("Type");

                    b.HasKey("Id");

                    b.HasIndex("InfoBookId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("GR.Database.Models.BookInfo", b =>
                {
                    b.Property<string>("BookId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("FavCount");

                    b.Property<string>("Intro");

                    b.Property<string>("Length");

                    b.Property<string>("LongDescription");

                    b.Property<string>("Press");

                    b.Property<string>("PushCount");

                    b.Property<string>("RecentUpdate");

                    b.Property<string>("Status");

                    b.Property<string>("StatusLong");

                    b.Property<string>("TodayHitCount");

                    b.Property<string>("TotalHitCount");

                    b.Property<string>("UpdateStatus");

                    b.HasKey("BookId");

                    b.ToTable("BookInfo");
                });

            modelBuilder.Entity("GR.Database.Models.Book", b =>
                {
                    b.HasOne("GR.Database.Models.BookInfo", "Info")
                        .WithMany()
                        .HasForeignKey("InfoBookId");
                });
        }
    }
}
