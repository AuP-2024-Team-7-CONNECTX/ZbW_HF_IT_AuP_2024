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

		// Wird von Put-API oder CommunicationController angestossen, wenn wir einen Zug erhalten vom Roboter oder Frontend.
		// Falls kein Spieler steuert, wird der Zug automatisch ausgeführt

		// Falls vom Frontend -> neuer Zug soll ausgeführt werden, Roboter wird informiert
		// Falls vom Roboter -> Zug vom Roboter
		public void ReceiveInput(int column, bool isAutomatic, bool IsFromFrontend)
		{
			_currentMove = _game.CurrentMove;
			// _gameField = _game.gameField; -> muss noch abgeglichen werden, wie koordinaten aussehen.

			// Algorithmus resp. methode, die berechnet wo der stein hin soll
			if (isAutomatic)
			{
				 //PlaceStone(column);
			}

			if (IsFromFrontend)
			{
				// Falls Zug vom Frontend und nicht algo-gesteuert, ist gamefield bereits aktualisiert und keine Aktion ist mehr nötig
				SendTurnToRobot();

			}
			else { SendTurnToFrontend(); }
		}

		public void SendTurnToRobot()
		{
			// API call zum Roboter, um ihm zug zu übermitteln
			// Erwartet: Response. falls ok, gut, falls nicht Fehlerhandling


		}
		public void SendTurnToFrontend()
		{
			// API call zum Frontend, um ihm zug zu übermitteln
			// Erwartet: Response. falls ok, gut, falls nicht Fehlerhandling

			// Danach muss Zug vom Frontend erhalten werden, 


		}

		// Wird nur ausgeführt, wenn Algorithmus die Logik des nächsten Zugs übernehmen soll
		// Ansonsten passiert aktualisierung des gameFields mit neuem zug direkt im Frontend
		public void PlaceNewStoneFromAlgorithm(int column)
		{
			var playerNumber = _currentMove.Player == _player1 ? 1 : 2;
			// Finde die erste leere Zeile in der angegebenen Spalte

			// hier column zuweisen aufgrund errechneter Wert mit Algorithmus -> muss noch implementiert werden

			for (int zeile = 0; zeile < _gameField[column].Count; zeile++)
			{
				if (_gameField[column][zeile] == 0)
				{
					// Setze den Stein des Spielers in das Feld
					_gameField[column][zeile] = playerNumber;

					return;
				}
			}
			throw new Exception($"Spalte {column} ist voll.");
		}


		private void SetUpPlayersAndRobots()
		{
			_robot1 = _game.Robots[0];
			_robot2 = _game.Robots[1];
			if (_robot1.CurrentPlayer != null)
			{
				_player1 = _robot1.CurrentPlayer;
				_robot1.ControlledByHuman = true;
			}
			else
			{
				_player1 = null;
				_robot1.ControlledByHuman = false;
			}

			if (_robot2.CurrentPlayer != null)
			{
				_player2 = _robot2.CurrentPlayer;
				_robot2.ControlledByHuman = true;
			}
			else
			{
				_player2 = null;
				_robot2.ControlledByHuman = false;
			}


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
			_moveRepository.CreateOrUpdateAsync(_currentMove);

			return true;
		}

		private void ChangeTurn()
		{
			_lastMove.MoveFinished = DateTime.Now;
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
			_moveRepository.CreateOrUpdateAsync(_lastMove);
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
				_logger.LogInformation(ex.Message);
			}

		}
	}
}
