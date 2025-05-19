using Microsoft.AspNetCore.Mvc;
using reSreLData.Models;
using Microsoft.AspNetCore.Authorization;
using reSreLData.Repositories;

namespace reSreL.Controllers
{
    public class CommentairesController : Controller
    {
        private readonly CommentaireRepository _commentaireRepository;
        private readonly UserRepository _userRepository;
        private readonly RessourceRepository _ressourceRepository;

        public CommentairesController(CommentaireRepository commentaireRepository, UserRepository userRepository, RessourceRepository ressourceRepository)
        {
            _commentaireRepository = commentaireRepository;
            _userRepository = userRepository;
            _ressourceRepository = ressourceRepository;
        }

        // GET: /Commentaires
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var commentaires = await _commentaireRepository.GetAllAsync();
            return View(commentaires);
        }


        // POST: /Commentaires/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // seulement pour utilisateurs connectés
        public async Task<IActionResult> Create(int ressourceId, string contenu)
        {
            if (string.IsNullOrWhiteSpace(contenu) || contenu.Length > 300)
            {
                TempData["ErreurCommentaire"] = "Le commentaire doit contenir entre 1 et 300 caractères.";
                return RedirectToAction("Details", "Ressources", new { id = ressourceId });
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            var commentaire = new Commentaire
            {
                Contenu = contenu,
                RessourceId = ressourceId,
                UserId = int.Parse(userIdClaim),
                Valide = false
            };

            await _commentaireRepository.CreateAsync(commentaire);
            TempData["MessageCommentaire"] = "Commentaire soumis à validation.";

            return RedirectToAction("Details", "Ressources", new { id = ressourceId });
        }

        // POST: /Commentaires/Valider/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Valider(int id)
        {
            var success = await _commentaireRepository.ValiderCommentaireAsync(id);
            if (!success) return NotFound();

            TempData["MessageValidation"] = "Commentaire validé.";
            return RedirectToAction("Index", "Ressources"); // ou où tu veux
        }

        // POST: /Commentaires/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _commentaireRepository.DeleteAsync(id);
            if (!success) return NotFound();

            TempData["MessageSuppression"] = "Commentaire supprimé.";
            return RedirectToAction("Index", "Ressources");
        }
    }
}
