using ConnectFour.Models;
using MQTTnet.Protocol;

public interface IMqttAndGameService
{
	Task ConnectToNewBrokerAsync(string brokerAddress, string brokerPort, string brokerUserName, string brokerPassword);
	Task SubscribeAsync(string brokerAddress, string port, string topic);
	Task<bool> PublishAsync(string brokerAddress, string port, string topic, string message, MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false);
	Task UnsubscribeAsync(string brokerAddress, string port, string topic);
	Task DisconnectAsync(string brokerAddress, string port);
	Task<Game> CreateNewGame(Game game);
	Task<Game> UpdateGame(Game game);
	Task<Game> StartGame(Game game);
	Task<Game> EndGame(Game game);
	Task<Game> AbortGame(Game game);
	Task<Game> ReceiveInput(Game game, string payload, bool isFromFrontend);
	Task<bool> SendTurnToRobot(Game game, string payload);
	void PlaceNewStoneFromAlgorithm(Game game, int column);
	IEnumerable<Game> GetAllGames();
	Game GetGameById(string id);
}