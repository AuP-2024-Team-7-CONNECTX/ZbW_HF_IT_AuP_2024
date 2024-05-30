using static ConnectFour.Enums.Enum;

public record RobotResponse
{
	public string Id { get; init; }
	public string? CurrentUserId { get; init; }
	public bool IsConnected { get; init; }
	public ConnectFourColor? Color { get; init; }
	public bool IsIngame { get; init; }
	public IEnumerable<string> GameIds { get; init; } // Vereinfachte Darstellung der Spiele
	public string Name { get; init; }
	public string BrokerAddress { get; set; }
	public int BrokerPort { get; set; }
	public string BrokerTopic { get; set; }
	public RobotResponse() { }
}
