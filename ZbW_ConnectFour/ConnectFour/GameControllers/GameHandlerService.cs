using ConnectFour.Models;
using ConnectFour.Mqtt;
using ConnectFour.Repositories.Interfaces;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.GameControllers
{
	public class GameHandlerService : IGameHandlerService
	{

		private readonly IMqttService _mqttService;
		private readonly ILogger<GameHandlerService> _logger;
		private readonly IGameRepository _gameRepository;

		public GameHandlerService(

			IMqttService mqttService,
			ILogger<GameHandlerService> logger,
			IGameRepository gameRepository)
		{

			_mqttService = mqttService ?? throw new ArgumentNullException(nameof(mqttService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_mqttService.MessageReceived += async (payload) => await OnMessageReceived(payload);
			_gameRepository = gameRepository;
		}

		private async Task<Game> OnMessageReceived(string payload)
		{
			var games = await _gameRepository.GetAllAsync();

			var currentGame = games.FirstOrDefault(g => g.State == Enums.Enum.GameState.InProgress);

			return await ReceiveInput(currentGame, payload, false);
		}

		public async Task<Game> CreateNewGame(Game game)
		{
			SetUpUsersAndRobots(game);
			return game;
		}

		public async Task<Game> UpdateGame(Game game)
		{
			return game;
		}

		public async Task<Game> StartGame(Game game)
		{
			await ConnectWithMqtt(game);
			return game;
		}

		public async Task<Game> EndGame(Game game)
		{
			game = ChangeIsIngameState(game, false, GameState.Completed);
			await DisconnectFromMqtt(game);
			return game;
		}

		public async Task<Game> AbortGame(Game game)
		{
			game = ChangeIsIngameState(game, false, GameState.Aborted);
			await DisconnectFromMqtt(game);
			return game;
		}

		public async Task<Game> ReceiveInput(Game game, string payload, bool isFromFrontend)
		{
			if (isFromFrontend)
			{
				if (game.ManualTurnIsAllowed)
				{
					if (!game.CurrentMove.Robot.ControlledByHuman)
					{
						var response = await SendTurnToRobot(game, payload);
					}

					game.RobotIsReadyForNextTurn = false;
					game.ManualTurnIsAllowed = false;
					game.NewTurnForFrontend = true;
					game.NewTurnForFrontendRowColumn = payload;
					return game;
				}
			}
			else
			{
				if (payload == "0")
				{
					game.RobotIsReadyForNextTurn = false;
					game.ManualTurnIsAllowed = false;
					return game;
				}

				if (payload == "1")
				{
					game.RobotIsReadyForNextTurn = true;
					game.ManualTurnIsAllowed = true;
					return game;
				}
			}

			return game;
		}

		public async Task<bool> SendTurnToRobot(Game game, string payload)
		{
			var success1 = await _mqttService.PublishAsync(game.Robots[0].BrokerAddress, game.Robots[0].BrokerPort.ToString(), $"{game.Robots[0].BrokerTopic}/coordinate", payload);
			var success2 = await _mqttService.PublishAsync(game.Robots[1].BrokerAddress, game.Robots[1].BrokerPort.ToString(), $"{game.Robots[1].BrokerTopic}/coordinate", payload);

			return success1 && success2;
		}

		public void PlaceNewStoneFromAlgorithm(Game game, int column)
		{
			throw new Exception($"Column {column} is full.");
		}

		private void SetUpUsersAndRobots(Game game)
		{
			game.Robots[0].ControlledByHuman = game.Robots[0].CurrentUser.Name.Contains("KI") ? false : true;
			game.Robots[1].ControlledByHuman = game.Robots[1].CurrentUser.Name.Contains("KI") ? false : true;
		}

		private async Task ConnectWithMqtt(Game game)
		{
			await _mqttService.SubscribeAsync(game.Robots[0].BrokerAddress, game.Robots[0].BrokerPort.ToString(), $"{game.Robots[0].BrokerTopic}/feedback");
			await _mqttService.SubscribeAsync(game.Robots[1].BrokerAddress, game.Robots[1].BrokerPort.ToString(), $"{game.Robots[1].BrokerTopic}/feedback");
		}

		private async Task DisconnectFromMqtt(Game game)
		{
			await _mqttService.UnsubscribeAsync(game.Robots[0].BrokerAddress, game.Robots[0].BrokerPort.ToString(), $"{game.Robots[0].BrokerTopic}/feedback");
			await _mqttService.UnsubscribeAsync(game.Robots[1].BrokerAddress, game.Robots[1].BrokerPort.ToString(), $"{game.Robots[1].BrokerTopic}/feedback");
		}

		private Game ChangeIsIngameState(Game game, bool isIngame, GameState gameState)
		{
			foreach (var robot in game.Robots)
			{
				robot.IsIngame = isIngame;
				robot.CurrentUser.IsIngame = isIngame;

			}

			game.State = gameState;
			return game;
		}


	}
}
