﻿// <auto-generated />
using System;
using AzFuncProcessReceipts.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AzFuncProcessReceipts.Model.Migrations
{
    [DbContext(typeof(ReceiptsContext))]
    [Migration("20231205174640_InitialDataMigration")]
    partial class InitialDataMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AzFuncProcessReceipts.Model.Receipt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("MerchantAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MerchantName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MerchantPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("SubTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Tip")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("TotalTax")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("TransactionDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Receipts");
                });

            modelBuilder.Entity("AzFuncProcessReceipts.Model.ReceiptItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Number")
                        .HasColumnType("int");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuantityUnity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ReceiptId")
                        .HasColumnType("int");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ReceiptId");

                    b.ToTable("ReceiptItem");
                });

            modelBuilder.Entity("AzFuncProcessReceipts.Model.TaxDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal?>("Ammount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("LineItem")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ReceiptId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReceiptId");

                    b.ToTable("TaxDetail");
                });

            modelBuilder.Entity("AzFuncProcessReceipts.Model.ReceiptItem", b =>
                {
                    b.HasOne("AzFuncProcessReceipts.Model.Receipt", null)
                        .WithMany("Items")
                        .HasForeignKey("ReceiptId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AzFuncProcessReceipts.Model.TaxDetail", b =>
                {
                    b.HasOne("AzFuncProcessReceipts.Model.Receipt", null)
                        .WithMany("TaxDetails")
                        .HasForeignKey("ReceiptId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AzFuncProcessReceipts.Model.Receipt", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("TaxDetails");
                });
#pragma warning restore 612, 618
        }
    }
}