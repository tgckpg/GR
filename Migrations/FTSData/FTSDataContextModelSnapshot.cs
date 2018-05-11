using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GR.Database.Contexts;

namespace GR.Migrations.FTSData
{
    [DbContext(typeof(FTSDataContext))]
    partial class FTSDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

            modelBuilder.Entity("GR.Database.Models.FTSChapter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChapterId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("FTSChapters");
                });
        }
    }
}
