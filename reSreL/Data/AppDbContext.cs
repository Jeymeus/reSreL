using Microsoft.EntityFrameworkCore;
using reSreL.Models;

namespace reSreL.Data
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ressource>()
                .HasMany(r => r.Categories)
                .WithMany(c => c.Ressources)
                .UsingEntity(j => j.ToTable("RessourceCategorie"));
        }

    }
}
