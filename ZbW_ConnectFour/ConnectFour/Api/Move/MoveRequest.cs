public record MoveRequest
{
	public string RobotId { get; init; }
	public string MoveDetails { get; init; }
	public float Duration { get; init; }
	public string GameId { get; init; }
	// overrides MoveDetails with algo-Turn
	public bool TurnWithAlgorithm { get; init; }
	public MoveRequest() { }
}
