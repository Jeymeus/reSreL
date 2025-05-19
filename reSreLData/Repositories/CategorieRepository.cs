using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Models;

namespace reSreLData.Repositories
{
    public class CategorieRepository
    {
        private readonly AppDbContext _context;

        public CategorieRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categorie>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Categorie?> GetByIdAsync(int id)
        {
            return await _context.Categories.Include(c => c.Ressources).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categorie> CreateAsync(Categorie categorie)
        {
            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();
            return categorie;
        }

        public async Task<bool> UpdateAsync(Categorie updated)
        {
            var categorie = await _context.Categories.FindAsync(updated.Id);
            if (categorie == null) return false;

            categorie.Nom = updated.Nom;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null) return false;

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
