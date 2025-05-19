using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using reSreLData.Models;
using reSreLData.Repositories;

namespace reSreL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CategorieRepository _categorieRepository;

        public HomeController(ILogger<HomeController> logger, CategorieRepository categorieRepository)
        {
            _logger = logger;
            _categorieRepository = categorieRepository;
        }

        private async Task SetSharedViewDataAsync()
        {
            ViewBag.Categories = await _categorieRepository.GetAllAsync();

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserId = User.FindFirst("UserId")?.Value;
            }
        }

        public async Task<IActionResult> Index()
        {
            await SetSharedViewDataAsync();
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            await SetSharedViewDataAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            await SetSharedViewDataAsync();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
