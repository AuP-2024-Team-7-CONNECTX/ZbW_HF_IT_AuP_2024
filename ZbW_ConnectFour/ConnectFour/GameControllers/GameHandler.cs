using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using System.Linq.Expressions;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.GameControllers
{
	public class GameHandler
	{
		private readonly IGameRepository _gameRepository;
		private readonly IPlayerRepository _playerRepository;
		private readonly IRobotRepository _robotRepository;
		private readonly ILogger<GameHandler> _logger;

		private Game _game;

		public GameHandler(IGameRepository gameRepository, IPlayerRepository playerRepository, IRobotRepository robotRepository, Game game, ILogger<GameHandler> logger)
		{
			_gameRepository = gameRepository;
			_playerRepository = playerRepository;
			_robotRepository = robotRepository;
			_game = game;
			_logger = logger;
		}

		public bool StartGame()
		{
			ChangeIsIngameState(true,GameState.InProgress);

			return true;
		}

		public bool EndGame()
		{
			ChangeIsIngameState(false,GameState.Completed);
			return true;
		}

		public bool AbortGame()
		{
			ChangeIsIngameState(false, GameState.Abandoned);
			return true;
		}

		private void ChangeIsIngameState(bool isIngame, GameState gameState)
		{
			try
			{
				foreach (var robot in _game.Robots)
				{
					robot.IsIngame = isIngame;
					var currentPlayer = robot.CurrentPlayer;
					currentPlayer.IsIngame = isIngame;
					_robotRepository.CreateOrUpdateAsync(robot);
					_playerRepository.CreateOrUpdateAsync(currentPlayer);
				}
				_game.State = gameState;
				_gameRepository.CreateOrUpdateAsync(_game);

			}
			catch (Exception ex)
			{
				_logger.LogInformation("");
			}

		}
	}
}
