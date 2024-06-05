public record MoveResponse
{
    public string Id { get; init; }
    public string PlayerId { get; init; }
    public string RobotId { get; init; }
    public string MoveDetails { get; init; }
    public float? Duration { get; init; }
    public string GameId { get; init; }

    public MoveResponse() { }
}
