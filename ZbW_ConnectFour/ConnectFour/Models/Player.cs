using ConnectFour.Interfaces;

namespace ConnectFour.Models
{
    public class Player : Entity
    {
        public string Name { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
         

        public Game? CurrentGame { get; set; }
    }
}
