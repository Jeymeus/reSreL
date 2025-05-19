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
                .WithMany() // ou .WithMany(u => u.Ressources) si tu ajoutes la collection côté User
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // ou Restrict si tu préfères empêcher la suppression
        }
    }
}
