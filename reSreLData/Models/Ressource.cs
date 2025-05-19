using System.ComponentModel.DataAnnotations;

namespace reSreLData.Models
{
    public class Ressource
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; } = "";

        [Required]
        public string Type { get; set; } = "";

        [Required]
        [Url]
        public string Lien { get; set; } = "";

        // relation many-to-many
        public List<Categorie> Categories { get; set; } = new();

        [Required]
        // Lien avec l'utilisateur
        public int UserId { get; set; } // Clé étrangère
        public User? User { get; set; }
    }
}
