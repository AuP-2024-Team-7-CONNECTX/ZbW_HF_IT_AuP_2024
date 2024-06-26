﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MQTTnet.Client;
using MQTTnet;
using System.Text;
using MQTTnet.Protocol;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ConnectFour.Tests
{
	[TestClass]
	public class GameControllerTest
	{

		[TestMethod]
		private async void test()
		{
			string broker = "test.mosquitto.org";
			int port = 1883;
			string clientId = Guid.NewGuid().ToString();
			string topic = "ConnectX/Testing/ComTest001";
			string username = "emqxtest";
			string password = "******";

			// Create a MQTT client factory
			var factory = new MqttFactory();

			// Create a MQTT client instance
			var mqttClient = factory.CreateMqttClient();

			// Create MQTT client options
			var options = new MqttClientOptionsBuilder()
				.WithTcpServer(broker, port) // MQTT broker address and port
											 //.WithCredentials(username, password) // Set username and password
				.WithClientId(clientId)
				.WithCleanSession()
				.Build();

			// Connect to MQTT broker
			var connectResult = await mqttClient.ConnectAsync(options);

			if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
			{
				Console.WriteLine("Connected to MQTT broker successfully.");

				// Subscribe to a topic
				await mqttClient.SubscribeAsync(topic);

				// Callback function when a message is received
				mqttClient.ApplicationMessageReceivedAsync += e =>
				{
					Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
					return Task.CompletedTask;
				};

				// Publish a message 10 times
				for (int i = 0; i < 10; i++)
				{
					var message = new MqttApplicationMessageBuilder()
						.WithTopic(topic)
						.WithPayload($"Hello, MQTT! Message number {i}")
						.WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
						.WithRetainFlag()
						.Build();

					await mqttClient.PublishAsync(message);
					await Task.Delay(1000); // Wait for 1 second
				}

				// Unsubscribe and disconnect
				await mqttClient.UnsubscribeAsync(topic);
				await mqttClient.DisconnectAsync();
			}
			else
			{
				Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
			}
		}
	}

}
