using ConnectFour.GameControllers;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;

namespace ConnectFour.Mqtt
{
	public class MqttService : IMqttService
	{
		private readonly ILogger<MqttService> _logger;
		private IMqttClient _mqttClient;
		public readonly IConfiguration _configuration;

		public GameHandler CurrentGameHandler;
		public MqttService(ILogger<MqttService> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
			_mqttClient = new MqttFactory().CreateMqttClient();

		}

		public async Task ConnectToNewBrokerAsync(string brokerAddress, string brokerPort, string brokerUserName, string brokerPassword)
		{
			var clientId = Guid.NewGuid().ToString();

			var options = new MqttClientOptionsBuilder()
				.WithTcpServer(brokerAddress, int.Parse(brokerPort))
				.WithClientId(clientId)
				//.WithCredentials(username, password)
				.WithCleanSession()
				.Build();

			var connectResult = await _mqttClient.ConnectAsync(options);

			if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
			{
				_logger.LogInformation($"Connected to MQTT broker {brokerAddress} successfully.");

			}
			else
			{
				_logger.LogError($"Failed to connect to MQTT broker {brokerAddress}: {connectResult.ResultCode}");
			}
		}

		public async Task ConnectAsync()
		{

			var brokerAddress = _configuration.GetSection("MqttBroker:Broker").Value;
			var brokerPort = _configuration.GetSection("MqttBroker:Port").Value;
			var brokerUserName = _configuration.GetSection("MqttBroker:Username").Value;
			var brokerPassword = _configuration.GetSection("MqttBroker:Password").Value;

			var clientId = Guid.NewGuid().ToString();

			var options = new MqttClientOptionsBuilder()
				.WithTcpServer(brokerAddress, int.Parse(brokerPort))
				.WithClientId(clientId)
				//.WithCredentials(username, password)
				.WithCleanSession()
				.Build();

			var connectResult = await _mqttClient.ConnectAsync(options);

			if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
			{
				_logger.LogInformation($"Connected to MQTT broker {brokerAddress} successfully.");

			}
			else
			{
				_logger.LogError($"Failed to connect to MQTT broker {brokerAddress}: {connectResult.ResultCode}");
			}
		}

		public async Task RegisterGameHandler(GameHandler gameHandler)
		{
			CurrentGameHandler = gameHandler;

			if (CurrentGameHandler == null)
			{
				_logger.LogError("GameHandler is null. Couldnt register Method ApplicationMessageReceivedAsync for MqttClient");

			}

			// Callback function when a message is received
			_mqttClient.ApplicationMessageReceivedAsync += e =>
			{
				var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
				CurrentGameHandler.ReceiveInput(payload, false, false);
				return Task.CompletedTask;
			};
		}


		public async Task SubscribeAsync(string topic)
		{
			await _mqttClient.SubscribeAsync(topic);
			_logger.LogInformation($"Subscribed to topic: {topic}");
		}

		public async Task PublishAsync(string topic, string message, MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false)
		{
			var mqttMessage = new MqttApplicationMessageBuilder()
				.WithTopic(topic)
				.WithPayload(message)
				.WithQualityOfServiceLevel(qosLevel)
				.WithRetainFlag(retainFlag)
				.Build();

			await _mqttClient.PublishAsync(mqttMessage);
			_logger.LogInformation($"Published message to topic '{topic}': {message}");
		}

		public async Task UnsubscribeAsync(string topic)
		{
			await _mqttClient.UnsubscribeAsync(topic);
			_logger.LogInformation($"Unsubscribed from topic: {topic}");
		}

		public async Task DisconnectAsync()
		{
			await _mqttClient.DisconnectAsync();
			_logger.LogInformation("Disconnected from MQTT broker.");
		}

		// Method is test only to
		public async Task RegisterTestConsoleLog()
		{
			_mqttClient.ApplicationMessageReceivedAsync += e =>
			{
				var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
				Console.WriteLine(payload);
				_logger.LogInformation(payload);
				return Task.CompletedTask;
			};
		}
	}
}