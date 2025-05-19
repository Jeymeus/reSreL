using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Models;

namespace reSreLData.Repositories
{
    public class CommentaireRepository
    {
        private readonly AppDbContext _context;

        public CommentaireRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Commentaire>> GetAllAsync()
        {
            return await _context.Commentaires
                .Include(c => c.Utilisateur)
                .Include(c => c.Ressource)
                .ToListAsync();
        }

        public async Task<Commentaire?> GetByIdAsync(int id)
        {
            return await _context.Commentaires
                .Include(c => c.Utilisateur)
                .Include(c => c.Ressource)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Commentaire>> GetByRessourceAsync(int ressourceId, bool uniquementValidés = true)
        {
            return await _context.Commentaires
                .Where(c => c.RessourceId == ressourceId && (!uniquementValidés || c.Valide))
                .Include(c => c.Utilisateur)
                .ToListAsync();
        }

        public async Task<List<Commentaire>> GetByRessourceIdValidOnlyAsync(int ressourceId)
        {
            return await _context.Commentaires
                .Where(c => c.RessourceId == ressourceId && c.Valide)
                .Include(c => c.Utilisateur)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }



        public async Task<Commentaire> CreateAsync(Commentaire commentaire)
        {
            commentaire.DateCreation = DateTime.Now;
            _context.Commentaires.Add(commentaire);
            await _context.SaveChangesAsync();
            return commentaire;
        }

        public async Task<bool> ValiderCommentaireAsync(int id)
        {
            var commentaire = await _context.Commentaires.FindAsync(id);
            if (commentaire == null) return false;

            commentaire.Valide = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var commentaire = await _context.Commentaires.FindAsync(id);
            if (commentaire == null) return false;

            _context.Commentaires.Remove(commentaire);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
