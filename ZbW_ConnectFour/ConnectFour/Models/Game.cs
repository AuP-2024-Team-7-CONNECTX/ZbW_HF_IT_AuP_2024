using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Models
{
    public class Game : Entity
    {
        public List<Player>? Players { get; set; }

        public List<Robot>? Robots { get; set; }


        [NotMapped]
        public Move? CurrentMove { get; set; }
        public string? CurrentMoveId { get; set; }

        [NotMapped]

        public Player? WinnerPlayer { get; set; }
        public string? WinnerPlayerId { get; set; }

        [NotMapped]
        public Robot? WinnerRobot { get; set; }
        public string? WinnerRobotId { get; set; }

        public GameState State { get; set; }

        [NotMapped]
        public decimal? TotalPointsPlayerOne { get; set; }
        public decimal? TotalPointsPlayerTwo { get; set; }

    }
}
