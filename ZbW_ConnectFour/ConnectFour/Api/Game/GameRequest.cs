using static ConnectFour.Enums.Enum;

public record GameRequest
{
    public string Id { get; init; }

    public List<string> PlayerIds { get; init; } = new List<string>();
    public List<string> RobotIds { get; init; } = new List<string>();
    public string? CurrentMoveId { get; init; }
    public string? WinnerPlayerId { get; init; }
    public string? WinnerRobotId { get; init; }
    public GameState State { get; init; }

    public GameRequest() { }
}
