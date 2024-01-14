﻿// <auto-generated />
using System;
using Calendar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Calendar.Migrations
{
    [DbContext(typeof(CalendarDbContext))]
    partial class CalendarDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Calendar.Models.Holidays", b =>
                {
                    b.Property<int>("HolidayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HolidayId"));

                    b.Property<DateTime>("HolidayDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("HolidayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HolidayId");

                    b.ToTable("Holidays");
                });

            modelBuilder.Entity("Calendar.Models.TaskGenerated", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("OriginalTaskId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("TaskGenerated");
                });

            modelBuilder.Entity("Calendar.Models.TaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BSEStatus")
                        .HasColumnType("int");

                    b.Property<int>("CDSLStatus")
                        .HasColumnType("int");

                    b.Property<int>("GeneratedTaskId")
                        .HasColumnType("int");

                    b.Property<int>("MCXStatus")
                        .HasColumnType("int");

                    b.Property<int>("NCDEXStatus")
                        .HasColumnType("int");

                    b.Property<int>("NSDLStatus")
                        .HasColumnType("int");

                    b.Property<int>("NSEStatus")
                        .HasColumnType("int");

                    b.Property<int>("OriginalTaskId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TaskStatus");
                });

            modelBuilder.Entity("Calendar.Models.Tasks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BusinessDays")
                        .HasColumnType("int");

                    b.Property<int>("DelaySubmissionBSE")
                        .HasColumnType("int");

                    b.Property<int>("DelaySubmissionCDSL")
                        .HasColumnType("int");

                    b.Property<int>("DelaySubmissionMCX")
                        .HasColumnType("int");

                    b.Property<int>("DelaySubmissionNCDEX")
                        .HasColumnType("int");

                    b.Property<int>("DelaySubmissionNSDL")
                        .HasColumnType("int");

                    b.Property<int>("DelaySubmissionNSE")
                        .HasColumnType("int");

                    b.Property<int>("DueCompletion")
                        .HasColumnType("int");

                    b.Property<int>("DueDays")
                        .HasColumnType("int");

                    b.Property<DateTime>("InactiveDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsBSE")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCDSL")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMCX")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNCDEX")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNSDL")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNSE")
                        .HasColumnType("bit");

                    b.Property<bool>("MarkInactive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NonSubmissionBSE")
                        .HasColumnType("int");

                    b.Property<int>("NonSubmissionCDSL")
                        .HasColumnType("int");

                    b.Property<int>("NonSubmissionMCX")
                        .HasColumnType("int");

                    b.Property<int>("NonSubmissionNCDEX")
                        .HasColumnType("int");

                    b.Property<int>("NonSubmissionNSDL")
                        .HasColumnType("int");

                    b.Property<int>("NonSubmissionNSE")
                        .HasColumnType("int");

                    b.Property<int>("RecurrenceFrequency")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TaskDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Calendar.Models.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
