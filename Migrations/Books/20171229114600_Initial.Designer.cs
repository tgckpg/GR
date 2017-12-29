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
    [Migration("20171229114600_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

            modelBuilder.Entity("GR.Database.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Description");

                    b.Property<byte>("TextLayout");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<byte>("Type");

                    b.Property<string>("ZItemId");

                    b.Property<string>("ZoneId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Title");

                    b.HasIndex("ZItemId");

                    b.HasIndex("ZoneId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("GR.Database.Models.BookInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<int>("BookId");

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

                    b.HasKey("Id");

                    b.HasIndex("BookId")
                        .IsUnique();

                    b.ToTable("BookInfo");
                });

            modelBuilder.Entity("GR.Database.Models.Chapter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BookId");

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Index");

                    b.Property<string>("Json_Meta");

                    b.Property<string>("Title");

                    b.Property<int>("VolumeId");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("VolumeId");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("GR.Database.Models.ChapterContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChapterId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId")
                        .IsUnique();

                    b.ToTable("ChapterContents");
                });

            modelBuilder.Entity("GR.Database.Models.ChapterImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChapterId");

                    b.Property<string>("Json_Urls");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId")
                        .IsUnique();

                    b.ToTable("ChapterImages");
                });

            modelBuilder.Entity("GR.Database.Models.Volume", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BookId");

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Index");

                    b.Property<string>("Json_Meta");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.ToTable("Volumes");
                });

            modelBuilder.Entity("GR.Database.Models.BookInfo", b =>
                {
                    b.HasOne("GR.Database.Models.Book", "Book")
                        .WithOne("Info")
                        .HasForeignKey("GR.Database.Models.BookInfo", "BookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.Database.Models.Chapter", b =>
                {
                    b.HasOne("GR.Database.Models.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GR.Database.Models.Volume", "Volume")
                        .WithMany("Chapters")
                        .HasForeignKey("VolumeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.Database.Models.ChapterContent", b =>
                {
                    b.HasOne("GR.Database.Models.Chapter", "Chapter")
                        .WithOne("Content")
                        .HasForeignKey("GR.Database.Models.ChapterContent", "ChapterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.Database.Models.ChapterImage", b =>
                {
                    b.HasOne("GR.Database.Models.Chapter", "Chapter")
                        .WithOne("Image")
                        .HasForeignKey("GR.Database.Models.ChapterImage", "ChapterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GR.Database.Models.Volume", b =>
                {
                    b.HasOne("GR.Database.Models.Book", "Book")
                        .WithMany("Volumes")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
