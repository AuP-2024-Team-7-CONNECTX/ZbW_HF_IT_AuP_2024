using static ConnectFour.Enums.Enum;

public record RobotRequest(
    string Id,
    string CurrentPlayerId,
    bool IsConnected,
    ConnectFourColor Color,
    bool IsIngame
);
