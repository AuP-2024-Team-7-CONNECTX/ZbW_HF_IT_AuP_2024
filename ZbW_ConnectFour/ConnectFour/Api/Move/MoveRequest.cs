public record MoveRequest
{
	public string RobotId { get; init; }
	public string MoveDetails { get; init; }
	public float Duration { get; init; }
	public string GameId { get; init; }

	public MoveRequest() { }
}
