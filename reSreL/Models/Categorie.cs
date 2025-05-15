using System.ComponentModel.DataAnnotations;

namespace reSreL.Models
{
    public class Categorie
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; } = "";

        // relation many-to-many
        public List<Ressource> Ressources { get; set; } = new();
    }
}
