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
    partial class BooksContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("ZItemId");

                    b.Property<string>("ZoneId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("InfoBookId");

                    b.HasIndex("Title");

                    b.HasIndex("ZItemId");

                    b.HasIndex("ZoneId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("GR.Database.Models.BookInfo", b =>
                {
                    b.Property<string>("BookId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("CoverSrcUrl");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("FavCount");

                    b.Property<string>("Json_Flags");

                    b.Property<string>("Json_Others");

                    b.Property<string>("LatestSection");

                    b.Property<string>("Length");

                    b.Property<string>("LongDescription");

                    b.Property<string>("OriginalUrl");

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

            modelBuilder.Entity("GR.Database.Models.Chapter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Index");

                    b.Property<string>("Title");

                    b.Property<string>("VolumeId");

                    b.HasKey("Id");

                    b.HasIndex("VolumeId");

                    b.ToTable("Chapter");
                });

            modelBuilder.Entity("GR.Database.Models.Volume", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BookId");

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Index");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.ToTable("Volume");
                });

            modelBuilder.Entity("GR.Database.Models.Book", b =>
                {
                    b.HasOne("GR.Database.Models.BookInfo", "Info")
                        .WithMany()
                        .HasForeignKey("InfoBookId");
                });

            modelBuilder.Entity("GR.Database.Models.Chapter", b =>
                {
                    b.HasOne("GR.Database.Models.Volume")
                        .WithMany("Chapters")
                        .HasForeignKey("VolumeId");
                });

            modelBuilder.Entity("GR.Database.Models.Volume", b =>
                {
                    b.HasOne("GR.Database.Models.Book")
                        .WithMany("Volumes")
                        .HasForeignKey("BookId");
                });
        }
    }
}
