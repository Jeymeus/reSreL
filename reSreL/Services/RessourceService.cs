using Microsoft.EntityFrameworkCore;
using reSreL.Data;
using reSreL.Models;

namespace reSreL.Services
{
    public class RessourceService
    {
        private readonly AppDbContext _context;

        public RessourceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ressource>> GetAllAsync()
        {
            return await _context.Ressources.ToListAsync();
        }

        public async Task<Ressource?> GetByIdAsync(int id)
        {
            return await _context.Ressources.FindAsync(id);
        }

        public async Task<Ressource> CreateAsync(Ressource ressource)
        {
            _context.Ressources.Add(ressource);
            await _context.SaveChangesAsync();
            return ressource;
        }

        public async Task<bool> UpdateAsync(Ressource updated)
        {
            var ressource = await _context.Ressources.FindAsync(updated.Id);
            if (ressource == null) return false;

            ressource.Nom = updated.Nom;
            ressource.Type = updated.Type;
            ressource.Lien = updated.Lien;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ressource = await _context.Ressources.FindAsync(id);
            if (ressource == null) return false;

            _context.Ressources.Remove(ressource);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Ressource>> GetAllWithCategoriesAndUserAsync()
        {
            return await _context.Ressources
                .Include(r => r.Categories)
                .Include(r => r.User)
                .ToListAsync();
        }

    }
}
