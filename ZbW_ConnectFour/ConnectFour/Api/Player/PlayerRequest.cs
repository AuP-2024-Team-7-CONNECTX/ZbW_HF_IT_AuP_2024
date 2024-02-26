using ConnectFour.Models;

namespace ConnectFour.Api.User
{

    public class PlayerRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public bool IsIngame { get; set; }
        
    }

}
