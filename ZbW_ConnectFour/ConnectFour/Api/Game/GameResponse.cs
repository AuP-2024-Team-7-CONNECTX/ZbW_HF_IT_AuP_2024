using ConnectFour.Api.User;
using ConnectFour.Models;
using static ConnectFour.Enums.Enum;

public record GameResponse
{
    public string Id { get; init; }
    public IEnumerable<UserResponse> Players { get; init; } = new List<UserResponse>();
    public IEnumerable<RobotResponse> Robots { get; init; } = new List<RobotResponse>();
    public string? CurrentMoveId { get; init; }
    public UserResponse? WinnerPlayer { get; init; }
    public RobotResponse? WinnerRobot { get; init; }
    public GameState State { get; init; }
    public decimal? TotalPointsPlayerOne { get; init; }
    public decimal? TotalPointsPlayerTwo { get; init; }

	public string GameFieldJson { get; init; }

    public bool ManualTurnIsAllowed { get; init; }

	public GameResponse() { }
}
