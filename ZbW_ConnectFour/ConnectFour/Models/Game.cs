using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Models
{
    public class Game : Entity
    {
        public virtual List<Player>? Players { get; set; }

        public virtual List<Robot>? Robots { get; set; }

        public virtual Move? CurrentMove { get; set; }
        public string? CurrentMoveId { get; set; }
        public virtual Player? WinnerPlayer { get; set; }
        public string? WinnerPlayerId { get; set; }
        public virtual Robot? WinnerRobot { get; set; }
        public string? WinnerRobotId { get; set; }

        public GameState State { get; set; }

        public decimal? TotalPointsPlayerOne { get; set; }
        public decimal? TotalPointsPlayerTwo { get; set; }

    }
}
