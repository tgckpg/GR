﻿using System;
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

            modelBuilder.Entity("GR.Database.Models.Anchor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BookId");

                    b.Property<int>("Index");

                    b.Property<string>("Json_Meta");

                    b.Property<string>("Ref0");

                    b.Property<string>("Ref1");

                    b.Property<string>("Ref2");

                    b.Property<byte>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.ToTable("Anrchors");
                });

            modelBuilder.Entity("GR.Database.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Description");

                    b.Property<bool>("Fav");

                    b.Property<string>("Json_Meta");

                    b.Property<DateTime?>("LastAccess");

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

                    b.Property<string>("DailyHitCount");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("FavCount");

                    b.Property<string>("Json_Flags");

                    b.Property<string>("Json_Others");

                    b.Property<string>("LastUpdateDate");

                    b.Property<string>("LatestSection");

                    b.Property<string>("Length");

                    b.Property<string>("LongDescription");

                    b.Property<string>("OriginalUrl");

                    b.Property<string>("PostingDate");

                    b.Property<string>("Press");

                    b.Property<string>("PushCount");

                    b.Property<string>("Status");

                    b.Property<string>("TotalHitCount");

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

                    b.Property<byte[]>("RawData");

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

            modelBuilder.Entity("GR.Database.Models.Anchor", b =>
                {
                    b.HasOne("GR.Database.Models.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade);
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
