using static ConnectFour.Enums.Enum;

public record RobotResponse(
    string Id,
    string CurrentPlayerId,
    bool IsConnected,
    ConnectFourColor Color,
    bool IsIngame
);
