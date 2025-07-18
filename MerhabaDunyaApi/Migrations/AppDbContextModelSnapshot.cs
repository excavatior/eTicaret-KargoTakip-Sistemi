﻿// <auto-generated />
using MerhabaDunyaApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MerhabaDunyaApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MerhabaDunyaApi.Models.EmissionFactor", b =>
                {
                    b.Property<int>("Kimlik")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Kimlik"));

                    b.Property<decimal>("KgCO2PerLitre")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("YakitTipi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Kimlik");

                    b.ToTable("EmissionFactors");
                });
#pragma warning restore 612, 618
        }
    }
}
