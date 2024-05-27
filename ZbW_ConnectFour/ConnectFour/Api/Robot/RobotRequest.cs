using static ConnectFour.Enums.Enum;

public record RobotRequest
{
	public string? CurrentUserId { get; init; }
	public bool IsConnected { get; init; }
	public bool IsIngame { get; init; }
	public string? Color { get; init; }
	public string Name { get; init; }
	public required string BrokerAddress { get; set; }
	public required int BrokerPort { get; set; }
	public RobotRequest() { }
}
