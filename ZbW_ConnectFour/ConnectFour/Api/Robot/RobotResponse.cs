﻿using static ConnectFour.Enums.Enum;

public record RobotResponse
{
    public string Id { get; init; }
    public string? CurrentPlayerId { get; init; }
    public bool IsConnected { get; init; }
    public ConnectFourColor Color { get; init; }
    public bool IsIngame { get; init; }
    public IEnumerable<string> GameIds { get; init; } // Vereinfachte Darstellung der Spiele

    public RobotResponse() { }
}
