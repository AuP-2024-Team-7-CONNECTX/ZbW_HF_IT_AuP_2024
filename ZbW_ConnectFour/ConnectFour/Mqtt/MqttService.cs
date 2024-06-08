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
		private IMqttClient _mqttClient1;
		private IMqttClient _mqttClient2;

		private string _mqttClient1BrokerAddress;
		private string _mqttClient1BrokerPort;
		private string _mqttClient2BrokerAddress;
		private string _mqttClient2BrokerPort;

		public readonly IConfiguration _configuration;

		public GameHandler CurrentGameHandler;

		public MqttService(ILogger<MqttService> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
			_mqttClient1 = new MqttFactory().CreateMqttClient();
			_mqttClient2 = new MqttFactory().CreateMqttClient();
		}

		private IMqttClient GetClient(string brokerAddress, string brokerPort)
		{
			if (_mqttClient1.IsConnected && _mqttClient1BrokerAddress == brokerAddress && _mqttClient1BrokerPort == brokerPort)
			{
				return _mqttClient1;
			}
			if (_mqttClient2.IsConnected && _mqttClient2BrokerAddress == brokerAddress && _mqttClient2BrokerPort == brokerPort)
			{
				return _mqttClient2;
			}
			return null;
		}

		public async Task ConnectToNewBrokerAsync(string brokerAddress, string brokerPort, string brokerUserName, string brokerPassword)
		{
			var client = GetClient(brokerAddress, brokerPort);

			if (client == null)
			{
				if (!_mqttClient1.IsConnected)
				{
					client = _mqttClient1;
					_mqttClient1BrokerAddress = brokerAddress;
					_mqttClient1BrokerPort = brokerPort;
				}
				else if (!_mqttClient2.IsConnected)
				{
					client = _mqttClient2;
					_mqttClient2BrokerAddress = brokerAddress;
					_mqttClient2BrokerPort = brokerPort;
				}
				else
				{
					_logger.LogError("Both MQTT clients are already connected to different brokers.");
					return;
				}
			}

			var clientId = Guid.NewGuid().ToString();
			var options = new MqttClientOptionsBuilder()
				.WithTcpServer(brokerAddress, int.Parse(brokerPort))
				.WithClientId(clientId)
				//.WithCredentials(brokerUserName, brokerPassword)
				.WithCleanSession()
				.Build();

			var connectResult = await client.ConnectAsync(options);

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
			CurrentGameHandler = gameHandler ?? throw new ArgumentNullException(nameof(gameHandler));

			_mqttClient1.ApplicationMessageReceivedAsync += e =>
			{
				var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
				CurrentGameHandler.ReceiveInput(payload, false);
				return Task.CompletedTask;
			};

			_mqttClient2.ApplicationMessageReceivedAsync += e =>
			{
				var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
				CurrentGameHandler.ReceiveInput(payload, false);
				return Task.CompletedTask;
			};
		}

		public async Task SubscribeAsync(string brokerAddress, string port, string topic)
		{
			var client = GetClient(brokerAddress, port);
			if (client == null)
			{
				_logger.LogError("No client is connected to the specified broker.");
				return;
			}

			await client.SubscribeAsync(topic);
			_logger.LogInformation($"Subscribed to broker:{brokerAddress}:{port} topic: {topic}");
		}

		public async Task PublishAsync(string brokerAddress, string port, string topic, string message, MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false)
		{
			var client = GetClient(brokerAddress, port);
			if (client == null)
			{
				_logger.LogError("No client is connected to the specified broker.");
				return;
			}

			var mqttMessage = new MqttApplicationMessageBuilder()
				.WithTopic(topic)
				.WithPayload(message)
				.WithQualityOfServiceLevel(qosLevel)
				.WithRetainFlag(retainFlag)
				.Build();

			await client.PublishAsync(mqttMessage);
			_logger.LogInformation($"Published message to topic '{topic}': {message}");
		}

		public async Task UnsubscribeAsync(string brokerAddress, string port, string topic)
		{
			var client = GetClient(brokerAddress, port);
			if (client == null)
			{
				_logger.LogError("No client is connected to the specified broker.");
				return;
			}

			await client.UnsubscribeAsync(topic);
			_logger.LogInformation($"Unsubscribed from topic: {topic}");
		}

		public async Task DisconnectAsync(string brokerAddress, string port)
		{
			if (_mqttClient1.IsConnected && _mqttClient1BrokerAddress == brokerAddress && _mqttClient1BrokerPort == port)
			{
				await _mqttClient1.DisconnectAsync();
				_logger.LogInformation("Client 1 disconnected from MQTT broker.");
				_mqttClient1BrokerAddress = null;
				_mqttClient1BrokerPort = null;
			}
			else if (_mqttClient2.IsConnected && _mqttClient2BrokerAddress == brokerAddress && _mqttClient2BrokerPort == port)
			{
				await _mqttClient2.DisconnectAsync();
				_logger.LogInformation("Client 2 disconnected from MQTT broker.");
				_mqttClient2BrokerAddress = null;
				_mqttClient2BrokerPort = null;
			}
			else
			{
				_logger.LogError("No client is connected to the specified broker address and port.");
			}
		}

		public async Task RegisterTestConsoleLog()
		{
			_mqttClient1.ApplicationMessageReceivedAsync += e =>
			{
				var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
				Console.WriteLine($"Client 1: {payload}");
				_logger.LogInformation($"Client 1: {payload}");
				return Task.CompletedTask;
			};

			_mqttClient2.ApplicationMessageReceivedAsync += e =>
			{
				var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
				Console.WriteLine($"Client 2: {payload}");
				_logger.LogInformation($"Client 2: {payload}");
				return Task.CompletedTask;
			};
		}
	}
}
