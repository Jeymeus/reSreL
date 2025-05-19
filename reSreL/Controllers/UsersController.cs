using Microsoft.AspNetCore.Mvc;
using reSreLData.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using reSreLData.Repositories;
using Microsoft.AspNetCore.Identity;


namespace reSreL.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserRepository _userRepository;

        public UsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        // GET: /Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.CreateAsync(user);
                return RedirectToAction(nameof(Login));
            }

            return View(user);
        }

        // GET: /Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var success = await _userRepository.UpdateAsync(user);
                return success ? RedirectToAction(nameof(Index)) : NotFound();
            }

            return View(user);
        }

        // GET: /Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _userRepository.DeleteAsync(id);
            return success ? RedirectToAction(nameof(Index)) : NotFound();
        }


        // GET: /Users/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string motDePasse)
        {
            var user = await _userRepository.AuthenticateAsync(email, motDePasse);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Identifiants invalides");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nom),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role) // ← ajoute cette ligne
            };


            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Users/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Users");
        }




    // GET : /Users/Profil
    public async Task<IActionResult> Profil()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null) return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim.Value);
            var user = await _userRepository.GetByIdAsync(userId);
            return View(user);

        }


        // GET : /Users/EditProfil
        public async Task<IActionResult> EditProfil()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null) return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim.Value);
            var user = await _userRepository.GetByIdAsync(userId);
            return View(user);
        }

        // POST : /Users/EditProfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfil(User updatedUser)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null) return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim.Value);
            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser == null) return NotFound();

            currentUser.Nom = updatedUser.Nom;
            currentUser.Prenom = updatedUser.Prenom;
            currentUser.Email = updatedUser.Email;

            await _userRepository.UpdateAsync(currentUser);

            return RedirectToAction("Profil");
        }


        // GET : /Users/ChangerMotDePasse
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string ancienMotDePasse, string nouveauMotDePasse, string confirmerMotDePasse)
        {
            if (nouveauMotDePasse != confirmerMotDePasse)
            {
                ModelState.AddModelError("", "Le nouveau mot de passe et sa confirmation ne correspondent pas.");
                return View();
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null) return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim.Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("", "Utilisateur introuvable.");
                return View();
            }

            // Vérifie le mot de passe actuel avec le PasswordHasher
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.MotDePasse, ancienMotDePasse);
            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Mot de passe actuel incorrect.");
                return View();
            }

            // Mets juste le nouveau mot de passe brut
            user.MotDePasse = nouveauMotDePasse;

            // C'est UpdateAsync qui hashera
            await _userRepository.UpdateAsync(user);

            return RedirectToAction("Profil");
        }

    }
}
