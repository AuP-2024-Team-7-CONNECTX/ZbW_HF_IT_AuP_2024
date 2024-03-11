using static ConnectFour.Enums.Enum;

public record GameRequest
{
    public string? CurrentMoveId { get; init; }
    // Active, Completed, Abandoned, InProgress
    public string State { get; init; }

    public GameRequest() { }
}
