﻿// <auto-generated />
using System;
using GoldCap.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GoldCap.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210716094832_SeedData")]
    partial class SeedData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GoldCap.Models.Expense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Expenses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Amount = 27m,
                            Category = 2,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 413, DateTimeKind.Local).AddTicks(4174),
                            Description = "First decscription"
                        },
                        new
                        {
                            Id = 2,
                            Amount = 14m,
                            Category = 6,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9508),
                            Description = "Second decscription"
                        },
                        new
                        {
                            Id = 3,
                            Amount = 35m,
                            Category = 7,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9556),
                            Description = "Third decscription"
                        },
                        new
                        {
                            Id = 4,
                            Amount = 8005m,
                            Category = 3,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9561),
                            Description = "Fourth decscription"
                        },
                        new
                        {
                            Id = 5,
                            Amount = 425m,
                            Category = 4,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9565),
                            Description = "Fifth decscription"
                        },
                        new
                        {
                            Id = 6,
                            Amount = 255m,
                            Category = 1,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9568),
                            Description = "Sixth decscription"
                        },
                        new
                        {
                            Id = 7,
                            Amount = 27m,
                            Category = 2,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9572),
                            Description = "First decscription"
                        },
                        new
                        {
                            Id = 8,
                            Amount = 14m,
                            Category = 6,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9575),
                            Description = "Second decscription"
                        },
                        new
                        {
                            Id = 9,
                            Amount = 35m,
                            Category = 7,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9578),
                            Description = "Third decscription"
                        },
                        new
                        {
                            Id = 10,
                            Amount = 8005m,
                            Category = 3,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9582),
                            Description = "Very long description to check if everything is ok-Very long description to check if everything is ok-Ending of the sentence somewhere here should brk"
                        },
                        new
                        {
                            Id = 11,
                            Amount = 425m,
                            Category = 4,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9585),
                            Description = "Fifth decscription"
                        },
                        new
                        {
                            Id = 12,
                            Amount = 255m,
                            Category = 1,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9588),
                            Description = "Sixth decscription"
                        },
                        new
                        {
                            Id = 13,
                            Amount = 27m,
                            Category = 2,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9591),
                            Description = "First decscription"
                        },
                        new
                        {
                            Id = 14,
                            Amount = 14m,
                            Category = 6,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9594),
                            Description = "Second decscription"
                        },
                        new
                        {
                            Id = 15,
                            Amount = 35m,
                            Category = 7,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9597),
                            Description = "Third decscription"
                        },
                        new
                        {
                            Id = 16,
                            Amount = 8005m,
                            Category = 3,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9601),
                            Description = "Fourth decscription"
                        },
                        new
                        {
                            Id = 17,
                            Amount = 425m,
                            Category = 4,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9604),
                            Description = "Fifth decscription"
                        },
                        new
                        {
                            Id = 18,
                            Amount = 255m,
                            Category = 1,
                            Date = new DateTime(2021, 7, 16, 11, 48, 32, 418, DateTimeKind.Local).AddTicks(9607),
                            Description = "Sixth decscription"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}