public record MqttRequest(
		string BrokerAddress,
		string Port,
		string Topic
	);