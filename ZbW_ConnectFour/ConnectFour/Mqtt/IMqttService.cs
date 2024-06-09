using MQTTnet.Protocol;

namespace ConnectFour.Mqtt
{
	public interface IMqttService
	{
		Task ConnectToNewBrokerAsync(string brokerAddress, string brokerPort, string brokerUserName, string brokerPassword);
		Task SubscribeAsync(string brokerAddress, string port, string topic);
		Task<bool> PublishAsync(string brokerAddress, string port, string topic, string message, MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false);
		Task UnsubscribeAsync(string brokerAddress, string port, string topic);
		Task DisconnectAsync(string brokerAddress, string port);
		Task RegisterGameHandler();
		Task RegisterTestConsoleLog();

		event Func<string, Task> MessageReceived;
	}
}
