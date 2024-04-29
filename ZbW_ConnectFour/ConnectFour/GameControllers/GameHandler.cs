using ConnectFour.Models;
using ConnectFour.Mqtt;
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
		private readonly IMqttService _mqttService;
		private readonly ILogger<GameHandler> _logger;

		private Game _game;
		private Move _currentMove;
		private Move? _lastMove;

		private Robot _robot1;
		private Robot _robot2;

		private Player _player1;
		private Player _player2;

		private static Random random = new Random();

		private bool RobotIsReadyForNextTurn = false;


		// 7 Horizontal, 6 vertikal Maximal
		private Dictionary<int, Dictionary<int, int>> _gameField;

		public GameHandler(IGameRepository gameRepository, IPlayerRepository playerRepository, IRobotRepository robotRepository, Game game, ILogger<GameHandler> logger, IMoveRepository moveRepository, IMqttService mqttService)
		{
			_gameRepository = gameRepository;
			_playerRepository = playerRepository;
			_moveRepository = moveRepository;
			_robotRepository = robotRepository;
			_game = game;
			_logger = logger;
			_mqttService = mqttService;

			_gameField = new Dictionary<int, Dictionary<int, int>>();

			// Erster Zug zuweisen
			_currentMove = _game.CurrentMove;

			StartGame();

		}
		public async void StartGame()
		{
			SetUpPlayersAndRobots();
			ConnectWithMqtt();
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
			await _moveRepository.CreateOrUpdateAsync(_currentMove);

		}



		// Wird von MQTTClient oder CommunicationController(Frontend) angestossen, wenn wir einen Zug erhalten.
		// Falls kein Spieler steuert, wird der Zug automatisch ausgeführt
		// Aktualisiertes, neues Spielprett erhalten wird vom frontend per PUT, deshalb ist das _currentGame bereits aktualisiert.

		// Falls vom Frontend -> neuer Zug soll ausgeführt werden, Roboter wird informiert
		// Falls vom Roboter -> Zug vom Roboter
		public void ReceiveInput(string payload, bool isAutomatic, bool IsFromFrontend)
		{
			_currentMove = _game.CurrentMove;

			// Frontend hat Zug gemacht und übermittelt uns den neuen Stand
			// Es wird nur der nächste Zug an den Roboter ermittelt, mehr machen wir nicht
			// Falls Zug vom Frontend und nicht algo-gesteuert, ist gamefield bereits aktualisiert und keine Aktion ist mehr nötig
			if (IsFromFrontend)
			{
				if (!RobotIsReadyForNextTurn)
				{
					// hier muss was passieren, damit das backend wartet und den nachfolgenden code erst ausführt, wenn der bool true ist.
				}
				_game.ManualTurnIsAllowed = false;


				// Algorithmus bestimmt

				if (!_currentMove.Robot.ControlledByHuman)
				{

					// Keine Änderung passiert -> Algorithmus bestimmt

					if (_gameField == _game._gameField)
					{
						// PlaceNewStoneFromAlgorithm()

						// Spielfeld von Game aktualisieren, dass frontend informiert ist.
						_game._gameField = _gameField;

					}
				}


				if (RobotIsReadyForNextTurn)
				{
					SendTurnToRobot();
				}
				else
				{
					// ... ?
				}

				return;

			}


			// Roboter schickt uns input
			// Inputs:
			// 0 - nicht bereit
			// 1 - bereit
			// -> Information an Frontend, dass roboter bereit / nicht bereit ist
			if (!IsFromFrontend)
			{
				// Rückmeldung von roboter, dass er nicht bereit ist für input
				if (payload == "0")
				{
					// InformFrontend, dass Board gesperrt ist.
					// Braucht es hier eine Methode
					RobotIsReadyForNextTurn = false;
					return;
				}

				if (payload == "1")
				{

					// InformFrontend, dass Board offen ist.
					RobotIsReadyForNextTurn = true;

					_game.ManualTurnIsAllowed = true;
					SendTurnToFrontend();
				}

			}



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

		public async void EndGame()
		{
			ChangeIsIngameState(false, GameState.Completed);

			DisconnectFromMqtt();

		}

		public void AbortGame()
		{
			ChangeIsIngameState(false, GameState.Abandoned);
			DisconnectFromMqtt();

		}


		private async void ConnectWithMqtt()
		{
			
			await _mqttService.ConnectAsync();
			await _mqttService.SubscribeAsync(_robot1.Topic);
			await _mqttService.SubscribeAsync(_robot2.Topic);
			await _mqttService.RegisterGameHandler(this);
		}


		private async void DisconnectFromMqtt()
		{
			await _mqttService.UnsubscribeAsync(_robot1.Topic);
			await _mqttService.UnsubscribeAsync(_robot2.Topic);
			await _mqttService.DisconnectAsync();
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
