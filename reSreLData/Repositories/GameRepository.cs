using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Models;

namespace reSreLData.Repositories
{
    public class GameRepository
    {
        private readonly AppDbContext _context;

        public GameRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await _context.Games
                .Include(g => g.CreatedBy)
                .Include(g => g.Opponent)
                .ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games
                .Include(g => g.CreatedBy)
                .Include(g => g.Opponent)
                .Include(g => g.Moves)
                    .ThenInclude(m => m.PlayedBy)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> CreateAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<bool> UpdateStatusAsync(int gameId, string newStatus)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null) return false;

            game.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetOpponentAsync(int gameId, int opponentId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null) return false;

            game.OpponentId = opponentId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetActiveUsersExceptAsync(int excludeUserId)
        {
            return await _context.Users
                .Where(u => u.Actif && u.Id != excludeUserId)
                .ToListAsync();
        }
    }
}
