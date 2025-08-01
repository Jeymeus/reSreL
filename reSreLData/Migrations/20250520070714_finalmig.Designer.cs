﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using reSreLData.Data;

#nullable disable

namespace reSreLData.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250520070714_finalmig")]
    partial class finalmig
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CategorieRessource", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("int");

                    b.Property<int>("RessourcesId")
                        .HasColumnType("int");

                    b.HasKey("CategoriesId", "RessourcesId");

                    b.HasIndex("RessourcesId");

                    b.ToTable("RessourceCategorie", (string)null);
                });

            modelBuilder.Entity("reSreLData.Models.Categorie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("reSreLData.Models.Commentaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenu")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime>("DateCreation")
                        .HasColumnType("datetime2");

                    b.Property<int>("RessourceId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("Valide")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("RessourceId");

                    b.HasIndex("UserId");

                    b.ToTable("Commentaires");
                });

            modelBuilder.Entity("reSreLData.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<int?>("OpponentId")
                        .HasColumnType("int");

                    b.Property<int>("RessourceId")
                        .HasColumnType("int");

                    b.Property<int?>("RessourceId1")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("OpponentId");

                    b.HasIndex("RessourceId")
                        .IsUnique();

                    b.HasIndex("RessourceId1")
                        .IsUnique()
                        .HasFilter("[RessourceId1] IS NOT NULL");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("reSreLData.Models.Move", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PlayedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("PlayedById")
                        .HasColumnType("int");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayedById");

                    b.ToTable("Moves");
                });

            modelBuilder.Entity("reSreLData.Models.Ressource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Lien")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Ressources");
                });

            modelBuilder.Entity("reSreLData.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Actif")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MotDePasse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prenom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CategorieRessource", b =>
                {
                    b.HasOne("reSreLData.Models.Categorie", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("reSreLData.Models.Ressource", null)
                        .WithMany()
                        .HasForeignKey("RessourcesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("reSreLData.Models.Commentaire", b =>
                {
                    b.HasOne("reSreLData.Models.Ressource", "Ressource")
                        .WithMany()
                        .HasForeignKey("RessourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("reSreLData.Models.User", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ressource");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("reSreLData.Models.Game", b =>
                {
                    b.HasOne("reSreLData.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("reSreLData.Models.User", "Opponent")
                        .WithMany()
                        .HasForeignKey("OpponentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("reSreLData.Models.Ressource", "Ressource")
                        .WithOne()
                        .HasForeignKey("reSreLData.Models.Game", "RessourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("reSreLData.Models.Ressource", null)
                        .WithOne("Game")
                        .HasForeignKey("reSreLData.Models.Game", "RessourceId1");

                    b.Navigation("CreatedBy");

                    b.Navigation("Opponent");

                    b.Navigation("Ressource");
                });

            modelBuilder.Entity("reSreLData.Models.Move", b =>
                {
                    b.HasOne("reSreLData.Models.Game", "Game")
                        .WithMany("Moves")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("reSreLData.Models.User", "PlayedBy")
                        .WithMany()
                        .HasForeignKey("PlayedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("PlayedBy");
                });

            modelBuilder.Entity("reSreLData.Models.Ressource", b =>
                {
                    b.HasOne("reSreLData.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("reSreLData.Models.Game", b =>
                {
                    b.Navigation("Moves");
                });

            modelBuilder.Entity("reSreLData.Models.Ressource", b =>
                {
                    b.Navigation("Game");
                });
#pragma warning restore 612, 618
        }
    }
}
