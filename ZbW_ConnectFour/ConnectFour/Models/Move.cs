using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour.Models
{
    public class Move : Entity
    {
        [NotMapped]
        public Player Player { get; set; }
        public string PlayerId { get; set; }
        [NotMapped]
        public Robot Robot { get; set; }
        public string RobotId { get; set; }

        public string MoveDetails { get; set; }

        public DateTime? MoveStarted { get; set; }
        public DateTime? MoveFinished { get; set; }

        // Duration in seconds
        public float? Duration { get; set; }
        [NotMapped]

        public Game Game { get; set; }
        public string GameId { get; set; }


    }
}
