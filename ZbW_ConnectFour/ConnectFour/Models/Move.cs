using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour.Models
{
    public class Move : Entity
    {
        
        public virtual User User { get; set; }
        public string PlayerId { get; set; }
       
        public virtual Robot Robot { get; set; }
        public string RobotId { get; set; }

        // Columns 1-7
        public string MoveDetails { get; set; }

        public DateTime? MoveStarted { get; set; }
        public DateTime? MoveFinished { get; set; }

        // Duration in seconds
        public float? Duration { get; set; }
     
        public bool TurnWithAlgorithm { get; set; }
        public virtual Game Game { get; set; }
        public string GameId { get; set; }


    }
}
