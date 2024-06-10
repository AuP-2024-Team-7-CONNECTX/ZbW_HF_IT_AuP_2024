using ConnectFour.Api.User;
using ConnectFour.Models;
using static ConnectFour.Enums.Enum;

public record GameResponse
{
	public string Id { get; init; }
	public IEnumerable<UserResponse> Users { get; init; } = new List<UserResponse>();
	public IEnumerable<RobotResponse> Robots { get; init; } = new List<RobotResponse>();
		public UserResponse? WinnerUser { get; init; }
	public RobotResponse? WinnerRobot { get; init; }
	public GameState State { get; init; }
	public decimal? TotalPointsPlayerOne { get; init; }
	public decimal? TotalPointsPlayerTwo { get; init; }

	public bool? NewTurnForFrontend {  get; init; }
	public string? NewTurnForFrontendRowColumn { get; init; }
	public string GameMode { get; init; }

	public bool ManualTurnIsAllowed { get; init; }

	public GameResponse() { }
}
