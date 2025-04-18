﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RealtorConnect.Data;

#nullable disable

namespace RealtorConnect.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Apartment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Area")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Floor")
                        .HasColumnType("integer");

                    b.Property<string>("HouseView")
                        .HasColumnType("text");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("RealtorGroupId")
                        .HasColumnType("integer");

                    b.Property<int>("RealtorId")
                        .HasColumnType("integer");

                    b.Property<int>("Rooms")
                        .HasColumnType("integer");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RealtorGroupId");

                    b.HasIndex("RealtorId");

                    b.ToTable("Apartments");
                });

            modelBuilder.Entity("Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Favorite", b =>
                {
                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<int>("ApartmentId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ClientId", "ApartmentId");

                    b.HasIndex("ApartmentId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("RealtorConnect.Models.ApartmentStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ApartmentStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Available"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Sold"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Rented"
                        });
                });

            modelBuilder.Entity("RealtorConnect.Models.GroupClient", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "ClientId");

                    b.HasIndex("ClientId");

                    b.ToTable("GroupClients");
                });

            modelBuilder.Entity("RealtorConnect.Models.GroupRealtor", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("RealtorId")
                        .HasColumnType("integer");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "RealtorId");

                    b.HasIndex("RealtorId");

                    b.ToTable("GroupRealtors");
                });

            modelBuilder.Entity("RealtorConnect.Models.Realtor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RealtorGroupId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RealtorGroupId");

                    b.ToTable("Realtors");
                });

            modelBuilder.Entity("RealtorConnect.Models.RealtorGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RealtorGroup");
                });

            modelBuilder.Entity("Apartment", b =>
                {
                    b.HasOne("RealtorConnect.Models.RealtorGroup", "RealtorGroup")
                        .WithMany("Apartments")
                        .HasForeignKey("RealtorGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealtorConnect.Models.Realtor", "Realtor")
                        .WithMany("Apartments")
                        .HasForeignKey("RealtorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realtor");

                    b.Navigation("RealtorGroup");
                });

            modelBuilder.Entity("Favorite", b =>
                {
                    b.HasOne("Apartment", "Apartment")
                        .WithMany("Favorites")
                        .HasForeignKey("ApartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Client", "Client")
                        .WithMany("Favorites")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Apartment");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("RealtorConnect.Models.GroupClient", b =>
                {
                    b.HasOne("Client", "Client")
                        .WithMany("GroupClients")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealtorConnect.Models.RealtorGroup", "RealtorGroup")
                        .WithMany("GroupClients")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("RealtorGroup");
                });

            modelBuilder.Entity("RealtorConnect.Models.GroupRealtor", b =>
                {
                    b.HasOne("RealtorConnect.Models.RealtorGroup", "RealtorGroup")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealtorConnect.Models.Realtor", "Realtor")
                        .WithMany("GroupRealtors")
                        .HasForeignKey("RealtorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realtor");

                    b.Navigation("RealtorGroup");
                });

            modelBuilder.Entity("RealtorConnect.Models.Realtor", b =>
                {
                    b.HasOne("RealtorConnect.Models.RealtorGroup", "RealtorGroup")
                        .WithMany("Realtors")
                        .HasForeignKey("RealtorGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RealtorGroup");
                });

            modelBuilder.Entity("Apartment", b =>
                {
                    b.Navigation("Favorites");
                });

            modelBuilder.Entity("Client", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("GroupClients");
                });

            modelBuilder.Entity("RealtorConnect.Models.Realtor", b =>
                {
                    b.Navigation("Apartments");

                    b.Navigation("GroupRealtors");
                });

            modelBuilder.Entity("RealtorConnect.Models.RealtorGroup", b =>
                {
                    b.Navigation("Apartments");

                    b.Navigation("GroupClients");

                    b.Navigation("Realtors");
                });
#pragma warning restore 612, 618
        }
    }
}
