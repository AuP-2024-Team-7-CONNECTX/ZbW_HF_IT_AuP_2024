namespace ConnectFour.Models
{
    public class Move
    {
        public string Id { get; set; }
        public User Player { get; set; } 
        public Robot Roboter { get; set; }
        public string MoveDetails { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public Game Game { get; set; }
        
    }
}
