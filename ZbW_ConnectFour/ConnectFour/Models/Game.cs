using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Models
{
    public class Game : Entity
    {
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Robot RobotOne { get; set; }
        public Robot RobotTwo { get; set; }
        public Move? CurrentMove { get; set; }
        public string? CurrentMoveId { get; set; }

        [NotMapped]
        public Player? Winner { get; set; }
        public GameState State { get; set; }

        [NotMapped]
        public List<Move>? MoveHistory { get; set; }
        public decimal? TotalPointsPlayerOne { get; set; }
        public decimal? TotalPointsPlayerTwo { get; set; }

    }
}
