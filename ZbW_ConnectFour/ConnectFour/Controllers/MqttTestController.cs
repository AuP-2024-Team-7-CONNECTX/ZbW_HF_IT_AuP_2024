using ConnectFour.Models;
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
		private JsonResponseMessage _responseJson;

		public MqttTestController(IMqttService mqttService, ILogger<MqttTestController> mqttTestController)
		{
			_mqttService = mqttService;
			_mqttTestController = mqttTestController;
		}

		[HttpPost("MqttConnectToBrokerAndTopic")]
		public async Task<IActionResult> MqttConnectToBrokerAndTopic([FromBody] MqttRequest request)
		{
			try
			{
				await _mqttService.ConnectToNewBrokerAsync(request.BrokerAddress, request.Port, "foo", "foo");
				await _mqttService.SubscribeAsync(request.Topic);
				await _mqttService.RegisterTestConsoleLog();

				_responseJson.Message = $"Verbindung zu Mqtt-Broker {request.BrokerAddress}:{request.Port}/{request.Topic} war erfolgreich";
				return StatusCode(200, _responseJson);

			}
			catch (Exception ex)
			{
				_mqttTestController.LogInformation($"Fehler beim Verbinden zu Mqtt-Broker: {request.BrokerAddress}:{request.Port}/{request.Topic} {ex.Message}");
				_mqttTestController.LogError($"Fehler beim Verbinden zu Mqtt-Broker: {request.BrokerAddress}:{request.Port}/{request.Topic} {ex.Message}");
				_responseJson.Message = $"Fehler beim Verbinden zu Mqtt-Broker: {request.BrokerAddress}:{request.Port}/{request.Topic} {ex.Message}";
				return StatusCode(500, _responseJson);
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
