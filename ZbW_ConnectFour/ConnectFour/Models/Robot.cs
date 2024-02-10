using ConnectFour.Interfaces;
using System.Drawing;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Models
{
    public class Robot : Entity
    {
        public Player? CurrentPlayer { get; set; }
        public bool IsConnected { get; set; }
       
        public ConnectFourColor Color { get; set; }

        public Game? CurrentGame { get; set; }
    }
}
