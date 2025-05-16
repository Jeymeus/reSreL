using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using reSreL.Models;
using reSreL.Services;

namespace reSreL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CategorieService _categorieService;

        public HomeController(ILogger<HomeController> logger, CategorieService categorieService)
        {
            _logger = logger;
            _categorieService = categorieService;
        }

        private async Task SetSharedViewDataAsync()
        {
            ViewBag.Categories = await _categorieService.GetAllAsync();

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
