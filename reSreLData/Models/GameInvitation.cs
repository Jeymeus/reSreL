using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace reSreLData.Models
{
    public enum InvitationStatus
    {
        EnAttente,
        Acceptee,
        Refusee
    }

    public class GameInvitation
    {
        [Key]
        public int Id { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public InvitationStatus Status { get; set; } = InvitationStatus.EnAttente;

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public Game Game { get; set; }

        [ForeignKey("InvitedUser")]
        public int InvitedUserId { get; set; }
        public User InvitedUser { get; set; }
    }
}
