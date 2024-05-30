using ConnectFour.Models;
using ConnectFour.Mqtt;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
	// Diese Klasse ist nur für Tests

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

			_responseJson = new JsonResponseMessage();
		}

		[HttpPost("MqttTestComplete")]
		public async Task<IActionResult> MqttTestComplete()
		{
			try
			{
				string brokerAddress = "mqtt.mon3y.ch";
				string port = "1883";
				string topic = "ConnectX/feedback";
				await _mqttService.ConnectToNewBrokerAsync(brokerAddress, port, "foo", "foo");
				await _mqttService.SubscribeAsync(brokerAddress, port, topic);
				await _mqttService.RegisterTestConsoleLog();

				_responseJson.Message = $"Verbindung und Abonnement zum Broker {brokerAddress}:{port}/{topic} war erfolgreich. Bitte veröffentlichen Sie etwas auf Ihrem Broker.";
				return StatusCode(200, _responseJson);
			}
			catch (Exception ex)
			{
				_mqttTestController.LogError($"Fehler beim Verbinden zum MQTT-Broker-Test");
				_responseJson.Message = $"Fehler beim Verbinden zum MQTT-Broker-Test";
				return StatusCode(500, _responseJson);
			}
		}

		[HttpPost("MqttConnectToBroker")]
		public async Task<IActionResult> MqttConnectToBroker([FromBody] MqttRequest request)
		{
			try
			{
				await _mqttService.ConnectToNewBrokerAsync(request.BrokerAddress, request.Port, "foo", "foo");
				await _mqttService.RegisterTestConsoleLog();

				_responseJson.Message = $"Verbindung zu Mqtt-Broker {request.BrokerAddress}:{request.Port}/{request.Topic} war erfolgreich.";
				return StatusCode(200, _responseJson);
			}
			catch (Exception ex)
			{
				_mqttTestController.LogError($"Fehler beim Verbinden zu Mqtt-Broker {request.BrokerAddress}:{request.Port}/{request.Topic}: {ex.Message}");
				_responseJson.Message = $"Fehler beim Verbinden zu Mqtt-Broker {request.BrokerAddress}:{request.Port}/{request.Topic}: {ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		[HttpPost("MqttSubscribe")]
		public async Task<IActionResult> MqttSubscribe([FromBody] MqttRequest request)
		{
			try
			{
				await _mqttService.SubscribeAsync(request.BrokerAddress, request.Port, request.Topic);

				_responseJson.Message = $"MQTT-Topic '{request.Topic}' bei Broker {request.BrokerAddress}:{request.Port} erfolgreich abonniert.";
				return StatusCode(200, _responseJson);
			}
			catch (Exception ex)
			{
				_responseJson.Message = $"Fehler beim Abonnieren vom MQTT-Topic '{request.Topic}' bei Broker {request.BrokerAddress}:{request.Port}: {ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		[HttpPost("MqttPublish")]
		public async Task<IActionResult> MqttPublish([FromBody] MqttRequest request)
		{
			try
			{
				await _mqttService.PublishAsync(request.BrokerAddress, request.Port, request.Topic, "5");

				_responseJson.Message = $"Nachricht erfolgreich im MQTT-Topic '{request.Topic}' bei Broker {request.BrokerAddress}:{request.Port} veröffentlicht.";
				return StatusCode(200, _responseJson);
			}
			catch (Exception ex)
			{
				_responseJson.Message = $"Fehler beim Veröffentlichen der Nachricht im MQTT-Topic '{request.Topic}' bei Broker {request.BrokerAddress}:{request.Port}: {ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		[HttpPost("MqttUnsubscribe")]
		public async Task<IActionResult> MqttUnsubscribe([FromBody] MqttRequest request)
		{
			try
			{
				await _mqttService.UnsubscribeAsync(request.BrokerAddress, request.Port, request.Topic);

				_responseJson.Message = $"Erfolgreich vom MQTT-Topic '{request.Topic}' bei Broker {request.BrokerAddress}:{request.Port} abgemeldet.";
				return StatusCode(200, _responseJson);
			}
			catch (Exception ex)
			{
				_responseJson.Message = $"Fehler beim Abmelden vom MQTT-Topic '{request.Topic}' bei Broker {request.BrokerAddress}:{request.Port}: {ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		[HttpPost("MqttDisconnect")]
		public async Task<IActionResult> MqttDisconnect([FromBody] MqttRequest request)
		{
			try
			{
				await _mqttService.DisconnectAsync(request.BrokerAddress, request.Port);

				_responseJson.Message = $"Verbindung zum Mqtt-Broker {request.BrokerAddress}:{request.Port} wurde erfolgreich getrennt.";
				return StatusCode(200, _responseJson);
			}
			catch (Exception ex)
			{
				_responseJson.Message = $"Beim Trennen der Verbindung zum Mqtt-Broker {request.BrokerAddress}:{request.Port} ist ein Fehler aufgetreten: {ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}
	}
}
