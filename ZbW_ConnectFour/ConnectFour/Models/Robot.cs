namespace ConnectFour.Models
{
    public class Robot
    {
        public string Id { get; set; }
        public User? CurrentPlayer { get; set; }
        public bool IsConnected { get; set; }

    }
}
