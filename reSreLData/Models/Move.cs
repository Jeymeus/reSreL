using reSreLData.Models;

namespace reSreLData.Models
{
    public class Move
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public int PlayedById { get; set; }
        public User PlayedBy { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public DateTime PlayedAt { get; set; } = DateTime.Now;
    }

}
