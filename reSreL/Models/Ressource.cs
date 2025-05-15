using System.ComponentModel.DataAnnotations;


namespace reSreL.Models
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
    }
}
