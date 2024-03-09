public record MoveRequest
{   
    public string PlayerId { get; init; }
    public string RobotId { get; init; }
    public string MoveDetails { get; init; }
    public DateTime? MoveStarted { get; init; }
    public DateTime? MoveFinished { get; init; }
    public string GameId { get; init; }

    public MoveRequest() { }
}
