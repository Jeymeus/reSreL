using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using reSreL.Data;
using reSreL.Models;

namespace reSreL.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
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
            user.MotDePasse = _passwordHasher.HashPassword(user, user.MotDePasse);
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

            // Hache le mot de passe si non vide
            if (!string.IsNullOrWhiteSpace(updatedUser.MotDePasse))
            {
                user.MotDePasse = _passwordHasher.HashPassword(user, updatedUser.MotDePasse);
            }

            user.Actif = updatedUser.Actif;
            user.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> AuthenticateAsync(string email, string motDePasse)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Actif);
            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.MotDePasse, motDePasse);
            return result == PasswordVerificationResult.Success ? user : null;
        }
    }
}
