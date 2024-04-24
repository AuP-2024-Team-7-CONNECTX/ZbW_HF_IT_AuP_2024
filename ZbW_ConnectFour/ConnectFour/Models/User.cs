using ConnectFour.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ConnectFour.Models
{
    public class User : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public bool Authenticated { get; set; }

	}
      
}
