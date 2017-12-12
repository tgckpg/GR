using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GR.Database.Contexts;
using GR.Database.Models;

namespace libwenku8.Migrations
{
    [DbContext(typeof(SettingsContext))]
    [Migration("20170720153824_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("GR.Database.Models.ContentReader", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("ContentReader");
                });

            modelBuilder.Entity("GR.Database.Models.Theme", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("Theme");
                });
        }
    }
}
