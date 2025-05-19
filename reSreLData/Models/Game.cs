using reSreLData.Models;


namespace reSreLData.Models
{
    public class Game
    {
        public int Id { get; set; }

        // Ajout du lien avec Ressource
        public int RessourceId { get; set; }
        public Ressource Ressource { get; set; } = null!;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public int? OpponentId { get; set; }
        public User? Opponent { get; set; }

        public string Status { get; set; } = "En attente";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Move> Moves { get; set; } = new();
    }

}
