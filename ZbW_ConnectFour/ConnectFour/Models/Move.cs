using ConnectFour.Interfaces;

namespace ConnectFour.Models
{
    public class Move : Entity
    {
        public Player Player { get; set; }
        public string PlayerId { get; set; }
        public Robot Robot { get; set; }
        public string RobotId { get; set; }

        public string MoveDetails { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan? Duration { get; set; }
        public Game Game { get; set; }


    }
}
