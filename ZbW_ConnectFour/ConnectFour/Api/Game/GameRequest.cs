using static ConnectFour.Enums.Enum;

public record GameRequest
{
	public List<string>? RobotIds { get; init; }
	public string State { get; init; }
	public string GameMode { get; init; }

	public bool? NewTurnForFrontend { get; init; }
	public string? NewTurnForFrontendRowColumn { get; init; }

	public bool? ManualTurnIsAllowed { get; init; }
}
