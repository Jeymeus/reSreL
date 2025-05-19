using System.ComponentModel.DataAnnotations;

namespace reSreLData.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasse { get; set; } = string.Empty;

        public bool Actif { get; set; } = true;

        [Required]
        public string Role { get; set; } = "User"; // ou "Admin" par défaut

    }
}
