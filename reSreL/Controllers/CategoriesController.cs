using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Models;

namespace reSreL.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        private async Task SetSharedViewDataAsync()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserId = User.FindFirst("UserId")?.Value;
            }
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            await SetSharedViewDataAsync();
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/PublicList
        [HttpGet]
        [AllowAnonymous] // facultatif si tout est public
        public async Task<IActionResult> PublicList()
        {
            var categories = await _context.Categories
                .Include(c => c.Ressources) // facultatif, si tu veux aussi les ressources
                .ToListAsync();

            return View(categories);
        }


        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            await SetSharedViewDataAsync();

            if (id == null) return NotFound();

            var categorie = await _context.Categories
                .Include(c => c.Ressources)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categorie == null) return NotFound();

            return View(categorie);
        }

        // GET: Categories/Create
        public async Task<IActionResult> Create()
        {
            await SetSharedViewDataAsync();
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom")] Categorie categorie)
        {
            await SetSharedViewDataAsync();

            if (ModelState.IsValid)
            {
                _context.Add(categorie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(categorie);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            await SetSharedViewDataAsync();

            if (id == null) return NotFound();

            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null) return NotFound();

            return View(categorie);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom")] Categorie categorie)
        {
            await SetSharedViewDataAsync();

            if (id != categorie.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categorie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategorieExists(categorie.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(categorie);
        }

        // GET: Categories/Delete/5
        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            await SetSharedViewDataAsync();

            if (id == null) return NotFound();

            var categorie = await _context.Categories
                .Include(c => c.Ressources) // 🔥 Charge les ressources liées
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categorie == null) return NotFound();

            return View(categorie);
        }


        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categorie = await _context.Categories
                .Include(c => c.Ressources)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categorie == null)
                return NotFound();

            if (categorie.Ressources.Any())
            {
                TempData["DeleteError"] = "❌ Impossible de supprimer : la catégorie est liée à des ressources.";
                return RedirectToAction(nameof(Delete), new { id }); // Retour sur la page de suppression
            }

            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool CategorieExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
