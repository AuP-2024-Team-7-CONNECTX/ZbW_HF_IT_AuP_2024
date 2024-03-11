using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour.Models
{
    public class Player : Entity
    {
        public string Name { get; set; }
        public virtual User User { get; set; }
        public string UserId { get; set; }
        public bool IsIngame { get; set; }

        public virtual List<Game> Games { get; set; }
    }
}
