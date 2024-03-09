using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour.Models
{
    public class Player : Entity
    {
        public string Name { get; set; }
        [NotMapped]

        public User User { get; set; }
        public string UserId { get; set; }

        [NotMapped]

        public bool IsIngame { get; set; }

        public List<Game> Games { get; set; }
    }
}
