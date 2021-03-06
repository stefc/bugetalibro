﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TXS.bugetalibro.Infrastructure.Persistence;

namespace TXS.bugetalibro.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(DataStoreContext))]
    partial class DataStoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7");

            modelBuilder.Entity("TXS.bugetalibro.Domain.Entities.Auszahlung", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<double>("Betrag")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Datum")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("KategorieId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notiz")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("KategorieId");

                    b.ToTable("Auszahlung");
                });

            modelBuilder.Entity("TXS.bugetalibro.Domain.Entities.Einzahlung", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<double>("Betrag")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Datum")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Einzahlung");
                });

            modelBuilder.Entity("TXS.bugetalibro.Domain.Entities.Kategorie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Kategorie");
                });

            modelBuilder.Entity("TXS.bugetalibro.Domain.Entities.Auszahlung", b =>
                {
                    b.HasOne("TXS.bugetalibro.Domain.Entities.Kategorie", "Kategorie")
                        .WithMany()
                        .HasForeignKey("KategorieId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
