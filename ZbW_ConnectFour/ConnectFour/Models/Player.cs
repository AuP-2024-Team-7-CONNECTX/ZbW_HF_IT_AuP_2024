namespace ConnectFour.Models
{
    public class Player
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public User User { get; set; }
        public bool IsIngame { get; set; }
        public bool IsOnTurn { get; set; }
        public bool Points { get; set; }

    }
}
