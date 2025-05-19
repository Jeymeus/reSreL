using System.Security.Claims;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Models;
using reSreLData.Repositories;


namespace reSreL.Controllers
{
    public class RessourcesController : Controller
    {
        private readonly RessourceRepository _ressourceRepository;
        private readonly CategorieRepository _categorieRepository;
        private readonly CommentaireRepository _commentaireRepository;
        private readonly GameRepository _gameRepository;
        private readonly AppDbContext _context;

        public RessourcesController(
            RessourceRepository ressourceRepository,
            CategorieRepository categorieRepository,
            CommentaireRepository commentaireRepository,
            GameRepository gameRepository,
            AppDbContext context)
        {
            _ressourceRepository = ressourceRepository;
            _categorieRepository = categorieRepository;
            _commentaireRepository = commentaireRepository;
            _gameRepository = gameRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? categorieId)
        {
            ViewData["NomSortParam"] = String.IsNullOrEmpty(sortOrder) ? "nom_desc" : "";

            var ressources = _context.Ressources
                .Include(r => r.Categories)
                .Include(r => r.User)
                .AsQueryable();

            // 🔍 Recherche sur le nom
            if (!string.IsNullOrEmpty(searchString))
            {
                ressources = ressources.Where(r => r.Nom.Contains(searchString));
            }

            // 📁 Filtrage par catégorie
            if (categorieId.HasValue)
            {
                ressources = ressources.Where(r => r.Categories.Any(c => c.Id == categorieId));
            }

            // 🔃 Tri par nom uniquement
            ressources = sortOrder switch
            {
                "nom_desc" => ressources.OrderByDescending(r => r.Nom),
                _ => ressources.OrderBy(r => r.Nom)
            };

            // 🔄 Données pour la vue
            ViewBag.Search = searchString;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SelectedCategorieId = categorieId;
            ViewBag.Categories = await _categorieRepository.GetAllAsync();

            return View(await ressources.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var ressource = await _context.Ressources
                .Include(r => r.Categories)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ressource == null) return NotFound();

            var commentaires = await _commentaireRepository.GetByRessourceIdValidOnlyAsync(id);

            ViewBag.Commentaires = commentaires;

            // Si c'est une activité, charger le Game associé
            bool isActivite = ressource.Categories.Any(c => c.Nom.ToLower() == "activité");

            if (isActivite)
            {
                var game = await _context.Games
                    .Include(g => g.CreatedBy)
                    .Include(g => g.Opponent)
                    .Include(g => g.Moves)
                    .FirstOrDefaultAsync(g => g.RessourceId == ressource.Id);

                ViewBag.Game = game;
                return View("DetailActivite", ressource);
            }

            return View(ressource); // vue par défaut (Detail.cshtml)
        }




        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categorieRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ressource ressource, int[] SelectedCategorieIds)
        {
            if (SelectedCategorieIds == null || SelectedCategorieIds.Length == 0)
            {
                ModelState.AddModelError("Categories", "Veuillez sélectionner au moins une catégorie.");
            }

            if (ModelState.IsValid)
            {
                // Associer les catégories
                ressource.Categories = await _context.Categories
                    .Where(c => SelectedCategorieIds.Contains(c.Id))
                    .ToListAsync();

                // Associer l'utilisateur connecté
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null) return Unauthorized();

                ressource.UserId = user.Id;

                // Sauvegarder la ressource
                await _ressourceRepository.CreateAsync(ressource);

                // 🧠 Création auto d'une Game si "activité"
                var isActivite = ressource.Categories.Any(c => c.Nom.ToLower() == "activité");
                if (isActivite)
                {
                    var game = new Game
                    {
                        CreatedById = user.Id,
                        RessourceId = ressource.Id,
                        Status = "En attente",
                        CreatedAt = DateTime.Now
                    };

                    _context.Games.Add(game);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(User.IsInRole("Admin") ? "Index" : "PublicList");
            }

            ViewBag.Categories = await _categorieRepository.GetAllAsync();
            return View(ressource);
        }

        public async Task<IActionResult> Edit(int id, string? source)
        {
            var ressource = await _ressourceRepository.GetByIdAsync(id);
            if (ressource == null) return NotFound();

            ViewBag.Categories = await _categorieRepository.GetAllAsync();
            ViewBag.SelectedCategorieIds = ressource.Categories.Select(c => c.Id).ToArray();

            ViewBag.Source = source ?? "Index"; // Défaut = admin

            return View(ressource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ressource ressource, int[] SelectedCategorieIds, string? source)
        {
            if (id != ressource.Id) return BadRequest();

            var existing = await _context.Ressources
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existing == null) return NotFound();

            // 🔐 Vérification des droits
            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            var isAdmin = User.IsInRole("Admin");
            var isOwner = currentUser != null && existing.UserId == currentUser.Id;

            if (!isAdmin && !isOwner)
                return Forbid(); // ⛔️ Interdiction si ni admin ni propriétaire

            if (ModelState.IsValid)
            {
                existing.Nom = ressource.Nom;
                existing.Type = ressource.Type;
                existing.Lien = ressource.Lien;

                existing.Categories = await _context.Categories
                    .Where(c => SelectedCategorieIds.Contains(c.Id))
                    .ToListAsync();

                await _context.SaveChangesAsync();
                return RedirectToAction(source ?? "Index"); // 👈 redirection dynamique
            }

            ViewBag.Categories = await _categorieRepository.GetAllAsync();
            ViewBag.SelectedCategorieIds = SelectedCategorieIds;
            ViewBag.Source = source ?? "Index";

            return View(ressource);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ressource = await _ressourceRepository.GetByIdAsync(id);
            return ressource == null ? NotFound() : View(ressource);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? returnToCategorieId)
        {
            var ressource = await _context.Ressources
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ressource == null) return NotFound();

            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            var isAdmin = User.IsInRole("Admin");
            var isOwner = currentUser != null && ressource.UserId == currentUser.Id;

            if (!isAdmin && !isOwner)
                return Forbid();

            _context.Ressources.Remove(ressource);
            await _context.SaveChangesAsync();

            // ✅ Redirige vers la suppression de la catégorie si demandé
            if (returnToCategorieId.HasValue)
            {
                return RedirectToAction("Delete", "Categories", new { id = returnToCategorieId.Value });
            }

            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> FiltrerParCategorie(int categorieId)
        {
            var ressources = await _context.Ressources
                .Include(r => r.Categories)
                .Where(r => r.Categories.Any(c => c.Id == categorieId))
                .ToListAsync();

            ViewBag.Categories = await _categorieRepository.GetAllAsync();
            ViewBag.CategorieId = categorieId;
            return View("Index", ressources);
        }


        public async Task<IActionResult> Filtre(int? categorieId)
        {
            var ressources = _context.Ressources.Include(r => r.Categories).AsQueryable();

            if (categorieId.HasValue)
            {
                ressources = ressources.Where(r => r.Categories.Any(c => c.Id == categorieId));
            }

            ViewBag.Categories = await _categorieRepository.GetAllAsync();
            ViewBag.SelectedCategorieId = categorieId;

            return View(await ressources.ToListAsync());
        }


        public async Task<IActionResult> PublicList(string? search, int? categorieId)
        {
            var ressources = _context.Ressources
                .Include(r => r.Categories)
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                ressources = ressources.Where(r =>
                    r.Nom.Contains(search) || r.Type.Contains(search));
            }

            if (categorieId.HasValue)
            {
                ressources = ressources.Where(r => r.Categories.Any(c => c.Id == categorieId));
            }

            ViewBag.Categories = await _categorieRepository.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.CategorieId = categorieId;

            return View(await ressources.ToListAsync());
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> InitialiserPartieAjax(int ressourceId)
        {
            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            if (user == null)
                return Json(new { success = false, message = "Utilisateur non connecté." });

            var ressourceExists = await _context.Ressources.AnyAsync(r => r.Id == ressourceId);
            if (!ressourceExists)
                return Json(new { success = false, message = "Ressource introuvable." });

            var existingGame = await _context.Games
                .FirstOrDefaultAsync(g => g.CreatedById == user.Id && g.RessourceId == ressourceId);

            if (existingGame != null)
                return Json(new { success = true, game = existingGame });

            var newGame = new Game
            {
                CreatedById = user.Id,
                RessourceId = ressourceId,
                Status = "En attente",
                CreatedAt = DateTime.Now
            };

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                game = new
                {
                    id = newGame.Id,
                    status = newGame.Status,
                    createdAt = newGame.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                }
            });
        }

        public async Task<IActionResult> DetailActivite(int id)
        {
            var ressource = await _context.Ressources
                .Include(r => r.User)
                .Include(r => r.Categories)
                .Include(r => r.Game)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ressource == null) return NotFound();

            // Exemple : récupération du jeu associé à l'utilisateur courant
            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            var game = await _context.Games
                .Include(g => g.Moves)
                .Include(g => g.CreatedBy)
                .Include(g => g.Opponent)
                .FirstOrDefaultAsync(g => g.CreatedById == currentUser.Id);

            ViewBag.Game = game;
            ViewBag.CanEdit = currentUser != null && (User.IsInRole("Admin") || ressource.UserId == currentUser.Id);
            ViewBag.Commentaires = await _context.Commentaires
                .Where(c => c.RessourceId == ressource.Id)
                .OrderByDescending(c => c.DateCreation)
                .ToListAsync();

            return View("DetailActivite", ressource);
        }


    }
}
