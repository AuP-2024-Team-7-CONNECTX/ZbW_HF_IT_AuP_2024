using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Models
{
    public class Robot : Entity
    {
        [NotMapped]

        public Player? CurrentPlayer { get; set; }
        public string? CurrentPlayerId { get; set; }

        public bool IsConnected { get; set; }

        public ConnectFourColor Color { get; set; }
        [NotMapped]
        public bool IsIngame { get; set; }

        public List<Game> Games { get; set; }


    }
}
