using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reSreL.Models
{
    public class Commentaire
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Contenu { get; set; } = string.Empty;

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        [Required]
        public bool Valide { get; set; } = false;

        // Relations
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User Utilisateur { get; set; }

        [Required]
        public int RessourceId { get; set; }
        [ForeignKey("RessourceId")]
        public Ressource Ressource { get; set; }
    }
}
