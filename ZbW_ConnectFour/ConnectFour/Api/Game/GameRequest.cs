using static ConnectFour.Enums.Enum;

public record GameRequest
{
	public List<string>? RobotIds { get; init; }
	public string? CurrentMoveId { get; init; }
	public string State { get; init; }

	public string GameMode { get; init; }


}
