using ConnectFour.Mqtt;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
	// This class is for testing only

	[ApiController]
	[Route("[controller]")]
	public class MqttTestController : ControllerBase
	{
		private readonly IMqttService _mqttService;
		private readonly ILogger<MqttTestController> _mqttTestController;

		public MqttTestController(IMqttService mqttService, ILogger<MqttTestController> mqttTestController)
		{
			_mqttService = mqttService;
			_mqttTestController = mqttTestController;
		}

		[HttpPost("MqttTestComplete")]
		[EnableCors("AllowAll")] // Erlaubt den Zugriff für alle Herkunftsorte
		public async Task<IActionResult> MqttTestComplete()
		{
			try
			{
				string brokerAddress = "test.mosquitto.org";
				string port = "1883";
				string topic = "MS3Test";
				await _mqttService.ConnectToNewBrokerAsync(brokerAddress, port, "foo", "foo");
				await _mqttService.SubscribeAsync(topic);
				await _mqttService.RegisterTestConsoleLog();

				// Setze CORS-Header in der Antwort
				Response.Headers.Add("Access-Control-Allow-Origin", "*"); // Erlaubt den Zugriff von allen Herkunftsorten
				Response.Headers.Add("Access-Control-Allow-Methods", "POST"); // Erlaubt POST-Anfragen
				Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type"); // Erlaubt bestimmte Header

				return Ok($"Connect / subscribe to broker {brokerAddress}:{port}/{topic} was successful. Please publish something on your broker");
			}
			catch (Exception ex)
			{
				_mqttTestController.LogInformation($"Failed to connect to MQTT broker: {ex.Message}");
				_mqttTestController.LogError($"Failed to connect to MQTT broker: {ex.Message}");
				return StatusCode(500, $"Failed to connect to MQTT broker: {ex.Message}");
			}
		}

		[HttpPost("MqttConnect")]
		public async Task<IActionResult> MqttConnect()
		{
			try
			{
				string brokerAddress = "test.mosquitto.org";
				string port = "1883";
				await _mqttService.ConnectToNewBrokerAsync(brokerAddress,port,"foo","foo");
				return Ok($"Connected to MQTT broker {brokerAddress}:{port} successfully.");
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
