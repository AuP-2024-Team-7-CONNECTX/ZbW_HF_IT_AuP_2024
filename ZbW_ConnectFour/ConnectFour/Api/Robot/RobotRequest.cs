using static ConnectFour.Enums.Enum;

public record RobotRequest
{
    public string? Id { get; init; }
    public string? CurrentPlayerId { get; init; }
    public bool IsConnected { get; init; }
    public ConnectFourColor Color { get; init; }
    public RobotRequest() { }
}
