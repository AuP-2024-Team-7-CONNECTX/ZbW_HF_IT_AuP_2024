using ConnectFour.GameControllers;
using MQTTnet.Protocol;

namespace ConnectFour.Mqtt
{
	public interface IMqttService
	{
		Task ConnectAsync();
		Task SubscribeAsync(string topic);
		Task PublishAsync(string topic, string message, MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false);
		Task UnsubscribeAsync(string topic);
		Task DisconnectAsync();
		Task RegisterGameHandler(GameHandler gameHandler);

		Task RegisterTestConsoleLog();

	}
}