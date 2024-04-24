using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using System;
using System.Linq.Expressions;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.GameControllers
{
	public class GameHandler
	{
		private readonly IGameRepository _gameRepository;
		private readonly IPlayerRepository _playerRepository;
		private readonly IRobotRepository _robotRepository;
		private readonly IMoveRepository _moveRepository;
		private readonly ILogger<GameHandler> _logger;

		private Game _game;
		private Move _currentMove;
		private Move? _lastMove;

		private Robot _robot1;
		private Robot _robot2;

		private Player _player1;
		private Player _player2;

		private static Random random = new Random();

		// 7 Horizontal, 6 vertikal Maximal
		private Dictionary<int, Dictionary<int, int>> _gameField;

		public GameHandler(IGameRepository gameRepository, IPlayerRepository playerRepository, IRobotRepository robotRepository, Game game, ILogger<GameHandler> logger, IMoveRepository moveRepository)
		{
			_gameRepository = gameRepository;
			_playerRepository = playerRepository;
			_robotRepository = robotRepository;
			_game = game;
			_logger = logger;

			// Erster Zug zuweisen
			_currentMove = _game.CurrentMove;
			SetUpPlayersAndRobots();
			_moveRepository = moveRepository;

			_gameField = new Dictionary<int, Dictionary<int, int>>();

		}

		
		public void PlaceStone(int column)
		{
			var playerNumber = _currentMove.Player == _player1 ? 0 : 1;
			// Finde die erste leere Zeile in der angegebenen Spalte
			for (int zeile = 5; zeile >= 0; zeile--)
			{
				if (_gameField[column][zeile] == 0)
				{
					// Setze den Stein des Spielers in das Feld
					_gameField[column][zeile] = playerNumber;
					// Hier kannst du weitere Logik für den Spielablauf implementieren
					return;
				}
			}
			// Wenn die Spalte voll ist, gibt es eine Fehlerbehandlung oder eine Benachrichtigung
		}



		private void SetUpPlayersAndRobots()
		{
			_robot1 = _game.Robots[0];
			_robot2 = _game.Robots[1];
			_player1 = _robot1.CurrentPlayer;
			_player2 = _robot2.CurrentPlayer;

		}

		public void RemoveGame(Game game)
		{
			// implement logic
		}

		public bool StartGame()
		{
			ChangeIsIngameState(true, GameState.InProgress);

			// First turn is decided randomly
			var beginner = random.Next(0, 2);

			if (beginner == 0)
			{
				_currentMove.Robot = _robot1;
				_currentMove.Player = _player1;
			}
			else
			{
				_currentMove.Robot = _robot2;
				_currentMove.Player = _player2;
			}

			return true;
		}

		private void ChangeTurn()
		{
			_lastMove = _currentMove;
			_currentMove = _game.CurrentMove;
			if (_currentMove.Player == _player1 && _currentMove.Robot == _robot1)
			{
				_currentMove.Robot = _robot2;
				_currentMove.Player = _player2;
			}
			else
			{
				_currentMove.Robot = _robot1;
				_currentMove.Player = _player1;
			}
			_moveRepository.CreateOrUpdateAsync(_currentMove);
			_gameRepository.CreateOrUpdateAsync(_game);
		}

		public bool EndGame()
		{
			ChangeIsIngameState(false, GameState.Completed);
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
