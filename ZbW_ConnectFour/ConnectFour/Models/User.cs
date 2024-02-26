using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour.Models
{
    public class User : Entity
    {
        public DateTime? DeletedOn { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Authenticated { get; set; }

        public User(string id,string name, string email, string password, bool authenticated)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Authenticated = authenticated;
        }
    }
}
