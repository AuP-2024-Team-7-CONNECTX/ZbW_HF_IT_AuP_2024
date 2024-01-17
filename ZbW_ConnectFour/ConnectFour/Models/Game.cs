using static ConnectFour.Enums.Enum;

namespace ConnectFour.Models
{
    public class Game
    {
        public string Id { get; set; }
        public List<Player> Players { get; set; }
        public List<Robot> Robots { get; set; }
        public Move CurrentMove { get; set; }
        public GameState State { get; set; } 
        public List<Move> MoveHistory { get; set; }
    }
}
