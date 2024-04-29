using ConnectFour.Mqtt;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
	// This class is for testing only

	[ApiController]
	[Route("[controller]")]
	public class MqttTestController : ControllerBase
	{
		private readonly IMqttService _mqttService;

		public MqttTestController(IMqttService mqttService)
		{
			_mqttService = mqttService;
		}

		[HttpPost("MqttConnect")]
		public async Task<IActionResult> MqttConnect()
		{
			try
			{
				await _mqttService.ConnectAsync();
				return Ok("Connected to MQTT broker successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to connect to MQTT broker: {ex.Message}");
			}
		}

		[HttpPost("MqttSubscribe")]
		public async Task<IActionResult> MqttSubscribe(string topic)
		{
			try
			{
				await _mqttService.SubscribeAsync(topic);

				return Ok($"Subscribed to MQTT topic '{topic}' successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to subscribe to MQTT topic '{topic}': {ex.Message}");
			}
		}

		[HttpPost("MqttRegisterConsoleLogging")]
		public async Task<IActionResult> TestConsoleLog()
		{
			try
			{
				await _mqttService.RegisterTestConsoleLog();

				return Ok($"register console loggin was successful");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to register console logging");
			}
		}

		[HttpPost("MqttPublish")]
		public async Task<IActionResult> MqttPublish(string topic)
		{
			try
			{
				await _mqttService.PublishAsync(topic, "Hello, MQTT! Message number 1");
				return Ok($"Message published to MQTT topic '{topic}' successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to publish message to MQTT topic '{topic}': {ex.Message}");
			}
		}

		[HttpPost("MqttUnsubscribe")]
		public async Task<IActionResult> MqttUnsubscribe(string topic)
		{
			try
			{
				await _mqttService.UnsubscribeAsync(topic);
				return Ok($"Unsubscribed from MQTT topic '{topic}' successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to unsubscribe from MQTT topic '{topic}': {ex.Message}");
			}
		}

		[HttpPost("MqttDisconnect")]
		public async Task<IActionResult> MqttDisconnect()
		{
			try
			{
				await _mqttService.DisconnectAsync();
				return Ok("Disconnected from MQTT broker.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to disconnect from MQTT broker: {ex.Message}");
			}
		}
	}
}
