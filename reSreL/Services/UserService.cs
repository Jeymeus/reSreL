using Microsoft.EntityFrameworkCore;
using reSreL.Data;
using reSreL.Models;

namespace reSreL.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(User updatedUser)
        {
            var user = await _context.Users.FindAsync(updatedUser.Id);
            if (user == null) return false;

            user.Nom = updatedUser.Nom;
            user.Prenom = updatedUser.Prenom;
            user.Email = updatedUser.Email;
            user.MotDePasse = updatedUser.MotDePasse;
            user.Actif = updatedUser.Actif;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
