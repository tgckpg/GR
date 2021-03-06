﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GR.Database.Contexts;
using GR.Database.Models;

namespace GR.Migrations.Settings
{
    [DbContext(typeof(SettingsContext))]
    partial class SettingsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

            modelBuilder.Entity("GR.Database.Models.BookInfoView", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("BookInfoView");
                });

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

            modelBuilder.Entity("GR.Database.Models.GRSystem", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("System");
                });

            modelBuilder.Entity("GR.Database.Models.GRTableConfig", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Json_Columns");

                    b.HasKey("Id");

                    b.ToTable("TableConfigs");
                });

            modelBuilder.Entity("GR.Database.Models.GRWidgetConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Json_Conf");

                    b.HasKey("Id");

                    b.ToTable("WidgetConfigs");
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
