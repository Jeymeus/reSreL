using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Models;
using System.Security.Claims;

namespace reSreL.Controllers
{
    public class GameController : Controller
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> StartSolo(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            game.Status = "En cours";
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Stop(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            game.Status = "Terminée";
            await _context.SaveChangesAsync();

            return Ok();
        }

        // 🔥 Nouveau : enregistrement du score depuis JS
        [HttpPost]
        public async Task<IActionResult> SaveResult([FromBody] GameResultDto result)
        {
            var game = await _context.Games.FindAsync(result.GameId);
            if (game == null) return NotFound();

            game.Status = "Terminée";

            // Tu peux aussi créer une table GameScore si tu veux persister tout ça
            Console.WriteLine($"Score : {result.UserScore} - {result.BotScore}");

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }

    public class GameResultDto
    {
        public int GameId { get; set; }
        public int UserScore { get; set; }
        public int BotScore { get; set; }
        public string Result { get; set; } = "";
    }
}
