using ConnectFour.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Collections.Concurrent;
using System.Text;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Mqtt
{
	public class MqttAndGameService : IMqttAndGameService
	{
		private readonly ILogger<MqttAndGameService> _logger;


		private static readonly ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>();

		private static readonly ConcurrentDictionary<string, bool> _brokersReady = new ConcurrentDictionary<string, bool>();

		private static int countReadyBroker1;
		private static int countReadyBroker2;

		

		public MqttAndGameService(ILogger<MqttAndGameService> logger)
		{
			_logger = logger;

			InitializeClients();
		}

		private void InitializeClients()
		{
			if (MqttClientHolder.MqttClient1 == null)
			{
				MqttClientHolder.MqttClient1 = new MqttFactory().CreateMqttClient();
				MqttClientHolder.MqttClient1.ApplicationMessageReceivedAsync += HandleApplicationMessageReceived;
			}

			if (MqttClientHolder.MqttClient2 == null)
			{
				MqttClientHolder.MqttClient2 = new MqttFactory().CreateMqttClient();
				MqttClientHolder.MqttClient2.ApplicationMessageReceivedAsync += HandleApplicationMessageReceived;
			}

			// Namen der beiden Clients
			string client1 = "Client1";
			string client2 = "Client2";

			// Beide Clients mit false als booleschen Wert in das Dictionary eintragen
			_brokersReady.TryAdd("Client1", false);
			_brokersReady.TryAdd("Client2", false);
		}

		public IEnumerable<Game> GetAllGames()
		{
			return _games.Values;
		}

		public Task<Game> GetGameById(string id)
		{
			_games.TryGetValue(id, out var game);
			return Task.FromResult(game);
		}


		private async Task HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
		{
			var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
			var currentGame = _games.Values.FirstOrDefault(g => g.State == GameState.InProgress);

			var clientId = e.ClientId;

			try
			{
				if (currentGame != null)
				{
					currentGame = await ReceiveInput(currentGame, payload, false, clientId);
					Console.WriteLine($"Payload {payload} received and registered for {currentGame.Id}");

				}
				else
				{
					_logger.LogWarning("No game in progress found.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("error on receiving and using Mqtt Message");

			}


		}

		private IMqttClient GetClient(string brokerAddress, string brokerPort)
		{
			if (MqttClientHolder.MqttClient1.IsConnected && MqttClientHolder.MqttClient1BrokerAddress == brokerAddress && MqttClientHolder.MqttClient1BrokerPort == brokerPort)
			{
				return MqttClientHolder.MqttClient1;
			}
			if (MqttClientHolder.MqttClient2.IsConnected && MqttClientHolder.MqttClient2BrokerAddress == brokerAddress && MqttClientHolder.MqttClient2BrokerPort == brokerPort)
			{
				return MqttClientHolder.MqttClient2;
			}
			return null;
		}

		public async Task ConnectToNewBrokerAsync(string brokerAddress, string brokerPort, string brokerUserName, string brokerPassword)
		{
			var client = GetClient(brokerAddress, brokerPort);

			if (client == null)
			{
				if (!MqttClientHolder.MqttClient1.IsConnected)
				{
					client = MqttClientHolder.MqttClient1;
					MqttClientHolder.MqttClient1BrokerAddress = brokerAddress;
					MqttClientHolder.MqttClient1BrokerPort = brokerPort;
				}
				else if (!MqttClientHolder.MqttClient2.IsConnected)
				{
					client = MqttClientHolder.MqttClient2;
					MqttClientHolder.MqttClient2BrokerAddress = brokerAddress;
					MqttClientHolder.MqttClient2BrokerPort = brokerPort;
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

		public async Task<bool> PublishAsync(string brokerAddress, string port, string topic, string message, MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce, bool retainFlag = false)
		{
			var client = GetClient(brokerAddress, port);
			if (client == null)
			{
				_logger.LogError("No client is connected to the specified broker.");
				return false;
			}

			var mqttMessage = new MqttApplicationMessageBuilder()
				.WithTopic(topic)
				.WithPayload(message)
				.WithQualityOfServiceLevel(qosLevel)
				.WithRetainFlag(retainFlag)
				.Build();

			await client.PublishAsync(mqttMessage);
			_logger.LogInformation($"Published message to broker:{brokerAddress}:{port}, topic '{topic}': {message}");

			return true;
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
			if (MqttClientHolder.MqttClient1.IsConnected && MqttClientHolder.MqttClient1BrokerAddress == brokerAddress && MqttClientHolder.MqttClient1BrokerPort == port)
			{
				await MqttClientHolder.MqttClient1.DisconnectAsync();
				_logger.LogInformation("Client 1 disconnected from MQTT broker.");
				MqttClientHolder.MqttClient1BrokerAddress = null;
				MqttClientHolder.MqttClient1BrokerPort = null;
			}
			else if (MqttClientHolder.MqttClient2.IsConnected && MqttClientHolder.MqttClient2BrokerAddress == brokerAddress && MqttClientHolder.MqttClient2BrokerPort == port)
			{
				await MqttClientHolder.MqttClient2.DisconnectAsync();
				_logger.LogInformation("Client 2 disconnected from MQTT broker.");
				MqttClientHolder.MqttClient2BrokerAddress = null;
				MqttClientHolder.MqttClient2BrokerPort = null;
			}
			else
			{
				_logger.LogError("No client is connected to the specified broker address and port.");
			}
		}

		public async Task<Game> CreateNewGame(Game game)
		{
			SetUpUsersAndRobots(game);
			_games[game.Id] = game;

			return game;
		}

		public async Task<Game> UpdateGame(Game game)
		{
			_games[game.Id] = game;

			return game;
		}

		public async Task<Game> StartGame(Game game)
		{
			_games[game.Id] = game;
			await ConnectWithMqtt(game);

			return game;
		}

		public async Task<Game> EndGame(Game game)
		{
			game = ChangeIsIngameState(game, false, GameState.Completed);
			//await DisconnectFromMqtt(game);
			_games[game.Id] = game;

			return game;
		}

		public async Task<Game> AbortGame(Game game)
		{
			game = ChangeIsIngameState(game, false, GameState.Aborted);
			//await DisconnectFromMqtt(game);
			_games[game.Id] = game;

			return game;
		}


		public async Task<Game> ReceiveInput(Game game, string payload, bool isFromFrontend, string clientId)
		{


			try
			{
				if (isFromFrontend)
				{
					if (game.State == GameState.Completed)
					{
						throw new InvalidOperationException("Spiel wurde bereits beendet. Es werden keine Züge mehr akzeptiert!");
					}
					//if (game.ManualTurnIsAllowed)
					//{
					int columnNumber;

					if (game.TurnWithAlgorithm)
					{
						var ai = new ConnectFourAI();
						var playerNumber = game.CurrentUserId == game.User1Id ? 1 : 2;

						columnNumber = ai.GetBestMove(game.GameField, playerNumber);
						payload = (columnNumber - 1).ToString();

						game.GameField.UpdateColumn(columnNumber, playerNumber);

						game.TurnColumnFromKI = columnNumber;
						game.SendFeedbackAfterPayloadReceiveAllowed = true;

						if (ai.CheckWin(game.GameField, playerNumber))
						{
							// Handle game win logic
							game.State = GameState.Completed;
							game.WinnerUserId = playerNumber == 1 ? game.User1Id : game.User2Id;

							int movesLeftForPlayer1 = ai.GetMovesLeft(game.GameField, 1);
							int movesLeftForPlayer2 = ai.GetMovesLeft(game.GameField, 2);

							game.TotalPointsUserOne = (game.WinnerUserId == game.User1Id) ? movesLeftForPlayer1 + 1 : movesLeftForPlayer1;
							game.TotalPointsUserTwo = (game.WinnerUserId == game.User2Id) ? movesLeftForPlayer1 + 1 : movesLeftForPlayer1;

							await SendTurnToRobot(game, game.TurnColumnFromKI.ToString());

							Thread.Sleep(8000);

							await SendTurnToRobot(game, "e");


						}

					}
					else
					{
						if (int.TryParse(payload, out columnNumber) && columnNumber >= 0 && columnNumber <= 6)
						{
							// 0 -> 1
							columnNumber++;
							var playerNumber = game.CurrentUserId == game.User1Id ? 1 : 2;
							game.GameField.UpdateColumn(columnNumber, playerNumber);

							var ai = new ConnectFourAI();
							if (ai.CheckWin(game.GameField, playerNumber))
							{
								// Handle game win logic
								game.State = GameState.Completed;
								game.WinnerUserId = playerNumber == 1 ? game.User1Id : game.User2Id;

								int movesLeftForPlayer1 = ai.GetMovesLeft(game.GameField, 1);

								game.TotalPointsUserOne = (game.WinnerUserId == game.User1Id) ? movesLeftForPlayer1 + 1 : movesLeftForPlayer1;
								game.TotalPointsUserTwo = (game.WinnerUserId == game.User2Id) ? movesLeftForPlayer1 + 1 : movesLeftForPlayer1;

							}
						}
						//}

						string payloadForRobot = columnNumber.ToString();

						if (game.State == GameState.Completed)
						{
							await SendTurnToRobot(game, "e");
						}
						else
						{
							await SendTurnToRobot(game, payloadForRobot);
						}

					}

					game.RobotIsReadyForNextTurn = false;
					game.ManualTurnIsAllowed = false;
					game.NewTurnForFrontend = false;
					game.NewTurnForFrontendRowColumn = payload;
					game.OverrideDbGameForGet = false;
					game.TurnWithAlgorithm = false;

					_games[game.Id] = game;
				}
				else
				{
					if (payload == "0")
					{
						if (clientId == MqttClientHolder.MqttClient1.Options.ClientId)
						{
							bool brokerReady = false;
							_brokersReady.AddOrUpdate("Client1", false, (key, oldValue) => brokerReady);

						}

						if (game.GameMode == GameMode.PlayerVsPlayer && clientId == MqttClientHolder.MqttClient2.Options.ClientId)
						{
							bool brokerReady = false;
							_brokersReady.AddOrUpdate("Client2", false, (key, oldValue) => brokerReady);

						}
						game.RobotIsReadyForNextTurn = false;
						game.ManualTurnIsAllowed = false;
						game.OverrideDbGameForGet = true;
						game.NewTurnForFrontend = false;

					}

					if (payload == "1" && game.NewTurnForFrontendRowColumn != null)
					{

						if (clientId == MqttClientHolder.MqttClient1.Options.ClientId)
						{
							bool brokerReady = false;
							if (payload == "1")
							{
								brokerReady = true;
							}
							_brokersReady.AddOrUpdate("Client1", false, (key, oldValue) => brokerReady);
							countReadyBroker1++;
						}

						if (game.GameMode == GameMode.PlayerVsPlayer)
						{
							if (clientId == MqttClientHolder.MqttClient2.Options.ClientId)
							{
								bool brokerReady = false;
								if (payload == "1")
								{
									brokerReady = true;
								}
								_brokersReady.AddOrUpdate("Client2", false, (key, oldValue) => brokerReady);
								countReadyBroker2++;

							}
						}


						if (game.GameMode == GameMode.PlayerVsPlayer)
						{
							if (_brokersReady["Client1"] && _brokersReady["Client2"] && countReadyBroker1 == 1 && countReadyBroker2 == 1)
							{

								game.RobotIsReadyForNextTurn = true;
								game.ManualTurnIsAllowed = true;
								game.NewTurnForFrontend = true;
								game.OverrideDbGameForGet = true;

								_brokersReady.AddOrUpdate("Client1", false, (key, oldValue) => false);
								_brokersReady.AddOrUpdate("Client2", false, (key, oldValue) => false);

								countReadyBroker1 = 0; 
								countReadyBroker2 = 0;
							}
						}

						if (game.GameMode == GameMode.PlayerVsRobot)
						{
							if (_brokersReady["Client1"] && countReadyBroker1 == 1)
							{

								game.RobotIsReadyForNextTurn = true;
								game.ManualTurnIsAllowed = true;
								game.NewTurnForFrontend = true;
								game.OverrideDbGameForGet = true;

								_brokersReady.AddOrUpdate("Client1", false, (key, oldValue) => false);
								

								Thread.Sleep(1500);
								if (game.State == GameState.Completed)
								{
									await SendTurnToRobot(game, "e");
									countReadyBroker1 = 0;
									countReadyBroker2 = 0;
								}
								else
								{
									if (game.SendFeedbackAfterPayloadReceiveAllowed)
									{
										await SendTurnToRobot(game, game.TurnColumnFromKI.ToString());
										game.SendFeedbackAfterPayloadReceiveAllowed = false;
									}
									
									countReadyBroker1 = 0;
									countReadyBroker2 = 0;
								}

							}
						}
					}

				}

				_games[game.Id] = game;

				return game;
			}
			catch (Exception e)
			{
				_logger.LogError($"Fehler beim Empfangen / Verschicken im Bezug auf Game / Mqtt: {e.Message}");
				Console.WriteLine($"Fehler beim Empfangen / Verschicken im Bezug auf Game / Mqtt: {e.Message}");
				throw new Exception($"Fehler beim Empfangen / Verschicken im Bezug auf Game / Mqtt: {e.Message}");
			}
		}


		public async Task<bool> SendTurnToRobot(Game game, string payload)
		{

			var success1 = await PublishAsync(game.Robots[0].BrokerAddress, game.Robots[0].BrokerPort.ToString(), $"{game.Robots[0].BrokerTopic}/coordinate", payload);
			var success2 = false;
			if (game.GameMode == GameMode.PlayerVsPlayer)
			{
				success2 = await PublishAsync(game.Robots[1].BrokerAddress, game.Robots[1].BrokerPort.ToString(), $"{game.Robots[1].BrokerTopic}/coordinate", payload);
			}
			else
			{
				success1 = true;
			}
			return success1 && success2;
		}

		public void PlaceNewStoneFromAlgorithm(Game game, int column)
		{
			throw new Exception($"Column {column} is full.");
		}

		private void SetUpUsersAndRobots(Game game)
		{
			game.Robots[0].ControlledByHuman = !game.Robots[0].CurrentUser.Name.Contains("KI");
			if (game.GameMode == GameMode.PlayerVsPlayer)
			{
				game.Robots[1].ControlledByHuman = !game.Robots[1].CurrentUser.Name.Contains("KI");

			}
		}

		private async Task ConnectWithMqtt(Game game)
		{
			await SubscribeAsync(game.Robots[0].BrokerAddress, game.Robots[0].BrokerPort.ToString(), $"{game.Robots[0].BrokerTopic}/feedback");
			if (game.GameMode == GameMode.PlayerVsPlayer)
			{
				await SubscribeAsync(game.Robots[1].BrokerAddress, game.Robots[1].BrokerPort.ToString(), $"{game.Robots[1].BrokerTopic}/feedback");
			}
		}

		private async Task DisconnectFromMqtt(Game game)
		{
			await UnsubscribeAsync(game.Robots[0].BrokerAddress, game.Robots[0].BrokerPort.ToString(), $"{game.Robots[0].BrokerTopic}/feedback");
			if (game.GameMode == GameMode.PlayerVsPlayer)
			{
				await UnsubscribeAsync(game.Robots[1].BrokerAddress, game.Robots[1].BrokerPort.ToString(), $"{game.Robots[1].BrokerTopic}/feedback");
			}
		}

		private Game ChangeIsIngameState(Game game, bool isIngame, GameState gameState)
		{
			game.State = gameState;
			return game;
		}

		public static class MqttClientHolder
		{
			public static IMqttClient MqttClient1 { get; set; }
			public static IMqttClient MqttClient2 { get; set; }
			public static string MqttClient1BrokerAddress { get; set; }
			public static string MqttClient1BrokerPort { get; set; }
			public static string MqttClient2BrokerAddress { get; set; }
			public static string MqttClient2BrokerPort { get; set; }
		}
	}
}
