using static ConnectFour.Enums.Enum;

public record GameRequest
{
    //public List<string>? PlayerIds { get; init; }
    public List<string>? RobotIds { get; init; }
    public string? CurrentMoveId { get; init; }
    public string State { get; init; }
    
}
