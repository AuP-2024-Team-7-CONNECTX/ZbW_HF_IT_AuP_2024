using static ConnectFour.Enums.Enum;

public record RobotRequest
{
    public string? CurrentPlayerId { get; init; }
    public bool IsConnected { get; init; }
    public bool IsIngame { get; init; }

    public string Color { get; init; }
       
    public string Name { get; init; }

    public required string Endpoint { get; init; }
    public RobotRequest() { }
}
