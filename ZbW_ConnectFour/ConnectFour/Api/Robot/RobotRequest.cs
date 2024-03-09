using static ConnectFour.Enums.Enum;

public record RobotRequest
{
    public string? CurrentPlayerId { get; init; }
    public bool IsConnected { get; init; }
    public string Color { get; init; }
       
    public RobotRequest() { }
}
