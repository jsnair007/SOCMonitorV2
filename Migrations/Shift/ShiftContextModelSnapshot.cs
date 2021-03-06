﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SOCMonitorV2.Models;

namespace SOCMonitorV2.Migrations.Shift
{
    [DbContext(typeof(ShiftContext))]
    partial class ShiftContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("SOCMonitorV2.Models.OfficialsModel", b =>
                {
                    b.Property<string>("ECNo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ECNo");

                    b.ToTable("tbl_Officials");
                });

            modelBuilder.Entity("SOCMonitorV2.Models.ShiftModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DutyDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DutyEndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DutyStartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ECNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShiftTimeBase")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("tbl_Shift");
                });
#pragma warning restore 612, 618
        }
    }
}
