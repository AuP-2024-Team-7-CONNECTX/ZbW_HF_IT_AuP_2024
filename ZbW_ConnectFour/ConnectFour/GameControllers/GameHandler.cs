using ConnectFour.Models;
using ConnectFour.Mqtt;
using ConnectFour.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.GameControllers
{
	public class GameHandler
	{
		private readonly IGameRepository _gameRepository;
		private readonly IUserRepository _userRepository;
		private readonly IRobotRepository _robotRepository;
		private readonly IMoveRepository _moveRepository;
		private readonly IMqttService _mqttService;
		private readonly ILogger<GameHandlerService> _logger;

		public Game _game;

		private Robot _robot1;
		private Robot _robot2;

		private static Random random = new Random();

		private bool RobotIsReadyForNextTurn = false;

		public GameHandler(IGameRepository gameRepository, IUserRepository userRepository, IRobotRepository robotRepository, Game game, ILogger<GameHandlerService> logger, IMoveRepository moveRepository, IMqttService mqttService)
		{
			_gameRepository = gameRepository;
			_userRepository = userRepository;
			_moveRepository = moveRepository;
			_robotRepository = robotRepository;
			_game = game;
			_logger = logger;
			_mqttService = mqttService;

			StartGame();
		}

		public async Task StartGame()
		{
			SetUpUsersAndRobots();
			await ConnectWithMqtt();
		}

		public async Task EndGame()
		{
			ChangeIsIngameState(false, GameState.Completed);
			await DisconnectFromMqtt();
		}

		public async Task AbortGame()
		{
			ChangeIsIngameState(false, GameState.Aborted);
			await DisconnectFromMqtt();
		}

		public async Task ReceiveInput(string payload, bool isFromFrontend)
		{
			if (isFromFrontend)
			{
				if (_game.ManualTurnIsAllowed)
				{
					if (!_game.CurrentMove.Robot.ControlledByHuman)
					{
						var response = await SendTurnToRobot(payload);
					}

					RobotIsReadyForNextTurn = false;
					_game.ManualTurnIsAllowed = false;
					_game.NewTurnForFrontend = true;
					_game.NewTurnForFrontendRowColumn = payload;
					await _gameRepository.CreateOrUpdateAsync(_game);
					return;
				}
			}
			else
			{
				if (payload == "0")
				{
					RobotIsReadyForNextTurn = false;
					_game.ManualTurnIsAllowed = false;
					await _gameRepository.CreateOrUpdateAsync(_game);
					return;
				}

				if (payload == "1")
				{
					RobotIsReadyForNextTurn = true;
					_game.ManualTurnIsAllowed = true;
					await _gameRepository.CreateOrUpdateAsync(_game);
					return;
				}
			}
		}

		public async Task<bool> SendTurnToRobot(string payload)
		{
			var success1 = await _mqttService.PublishAsync(_robot1.BrokerAddress, _robot1.BrokerPort.ToString(), $"{_robot1.BrokerTopic}/coordinate", payload);
			var success2 = await _mqttService.PublishAsync(_robot2.BrokerAddress, _robot2.BrokerPort.ToString(), $"{_robot2.BrokerTopic}/coordinate", payload);

			var wasSuccessful = (success1 && success2);
			return wasSuccessful;
		}

		public void PlaceNewStoneFromAlgorithm(int column)
		{
			throw new Exception($"Column {column} is full.");
		}

		private void SetUpUsersAndRobots()
		{
			_robot1 = _game.Robots[0];
			_robot2 = _game.Robots[1];

			_robot1.ControlledByHuman = !_robot1.CurrentUser.Name.Contains("KI");
			_robot2.ControlledByHuman = !_robot2.CurrentUser.Name.Contains("KI");
		}

		public void RemoveGame(Game game)
		{
			// implement logic
		}

		private async Task ConnectWithMqtt()
		{
			await _mqttService.SubscribeAsync(_robot1.BrokerAddress, _robot1.BrokerPort.ToString(), $"{_robot1.BrokerTopic}/feedback");
			await _mqttService.SubscribeAsync(_robot2.BrokerAddress, _robot2.BrokerPort.ToString(), $"{_robot2.BrokerTopic}/feedback");
			await _mqttService.RegisterGameHandler();
		}

		private async Task DisconnectFromMqtt()
		{
			await _mqttService.UnsubscribeAsync(_robot1.BrokerAddress, _robot1.BrokerPort.ToString(), $"{_robot1.BrokerTopic}/feedback");
			await _mqttService.UnsubscribeAsync(_robot2.BrokerAddress, _robot2.BrokerPort.ToString(), $"{_robot2.BrokerTopic}/feedback");
		}

		private async void ChangeIsIngameState(bool isIngame, GameState gameState)
		{
			try
			{
				foreach (var robot in _game.Robots)
				{
					robot.IsIngame = isIngame;
					var currentUser = robot.CurrentUser;
					currentUser.IsIngame = isIngame;
					await _robotRepository.CreateOrUpdateAsync(robot);
					await _userRepository.CreateOrUpdateAsync(currentUser);
				}
				_game.State = gameState;
				await _gameRepository.CreateOrUpdateAsync(_game);
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex.Message);
			}
		}
	}
}
