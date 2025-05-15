using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reSreL.Data;
using reSreL.Models;
using reSreL.Services;

namespace reSreL.Controllers
{
    public class RessourcesController : Controller
    {
        private readonly RessourceService _ressourceService;
        private readonly CategorieService _categorieService;
        private readonly AppDbContext _context;

        public RessourcesController(
            RessourceService ressourceService,
            CategorieService categorieService,
            AppDbContext context)
        {
            _ressourceService = ressourceService;
            _categorieService = categorieService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ressources = await _ressourceService.GetAllAsync();
            return View(ressources);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categorieService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ressource ressource, int[] SelectedCategorieIds)
        {
            if (ModelState.IsValid)
            {
                ressource.Categories = await _context.Categories
                    .Where(c => SelectedCategorieIds.Contains(c.Id))
                    .ToListAsync();

                await _ressourceService.CreateAsync(ressource);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _categorieService.GetAllAsync();
            return View(ressource);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ressource = await _ressourceService.GetByIdAsync(id);
            if (ressource == null) return NotFound();

            ViewBag.Categories = await _categorieService.GetAllAsync();
            ViewBag.SelectedCategorieIds = ressource.Categories.Select(c => c.Id).ToArray();

            return View(ressource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ressource ressource, int[] SelectedCategorieIds)
        {
            if (id != ressource.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = await _context.Ressources
                    .Include(r => r.Categories)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (existing == null) return NotFound();

                existing.Nom = ressource.Nom;
                existing.Type = ressource.Type;
                existing.Lien = ressource.Lien;

                existing.Categories = await _context.Categories
                    .Where(c => SelectedCategorieIds.Contains(c.Id))
                    .ToListAsync();

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _categorieService.GetAllAsync();
            ViewBag.SelectedCategorieIds = SelectedCategorieIds;
            return View(ressource);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ressource = await _ressourceService.GetByIdAsync(id);
            return ressource == null ? NotFound() : View(ressource);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _ressourceService.DeleteAsync(id);
            return success ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
