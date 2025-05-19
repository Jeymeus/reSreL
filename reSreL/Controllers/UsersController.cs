using Microsoft.AspNetCore.Mvc;
using reSreLData.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using reSreLData.Repositories;


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
        // GET: /Users
        public async Task<IActionResult> Index(string searchString, bool? isActive, string sortOrder)
        {
            var users = (await _userRepository.GetAllAsync()).AsQueryable();

            // 🔍 Recherche
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                users = users.Where(u =>
                    (!string.IsNullOrEmpty(u.Nom) && u.Nom.ToLower().Contains(searchString)) ||
                    (!string.IsNullOrEmpty(u.Prenom) && u.Prenom.ToLower().Contains(searchString)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(searchString))
                );
            }

            // 🟢 Filtre Actif/Inactif
            if (isActive.HasValue)
            {
                users = users.Where(u => u.Actif == isActive.Value);
            }

            // 🔃 Tri
            ViewData["NomSortParam"] = string.IsNullOrEmpty(sortOrder) ? "nom_desc" : "";
            users = sortOrder switch
            {
                "nom_desc" => users.OrderByDescending(u => u.Nom),
                _ => users.OrderBy(u => u.Nom)
            };

            // ViewBag pour garder les filtres actifs dans la vue
            ViewBag.Search = searchString;
            ViewBag.IsActive = isActive;

            return View(users.ToList());
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

        [HttpPost]
        public async Task<IActionResult> ToggleActif(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Actif = !user.Actif;
            var success = await _userRepository.UpdateAsync(user);

            if (!success)
                return StatusCode(500);

            return Json(new { actif = user.Actif });
        }

    }

}
