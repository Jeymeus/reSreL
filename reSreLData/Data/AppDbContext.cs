using Microsoft.EntityFrameworkCore;
using reSreLData.Models;

namespace reSreLData.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ressource> Ressources { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Commentaire> Commentaires { get; set; }

        // 👉 Ajout pour le Morpion
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation many-to-many entre Ressource et Categorie
            modelBuilder.Entity<Ressource>()
                .HasMany(r => r.Categories)
                .WithMany(c => c.Ressources)
                .UsingEntity(j => j.ToTable("RessourceCategorie"));

            // Relation many-to-one entre Ressource et User
            modelBuilder.Entity<Ressource>()
                .HasOne(r => r.User)
                .WithMany() // ou .WithMany(u => u.Ressources)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 👉 Relation Game - User (CreatedBy)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.CreatedBy)
                .WithMany()
                .HasForeignKey(g => g.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // 👉 Relation Game - User (Opponent)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Opponent)
                .WithMany()
                .HasForeignKey(g => g.OpponentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 👉 Relation Move - Game
            modelBuilder.Entity<Move>()
                .HasOne(m => m.Game)
                .WithMany(g => g.Moves)
                .HasForeignKey(m => m.GameId);

            // 👉 Relation Move - User (PlayedBy)
            modelBuilder.Entity<Move>()
                .HasOne(m => m.PlayedBy)
                .WithMany()
                .HasForeignKey(m => m.PlayedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Game - Ressource
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Ressource)
                .WithOne() // ou .WithMany() si plusieurs games pour une même ressource
                .HasForeignKey<Game>(g => g.RessourceId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
