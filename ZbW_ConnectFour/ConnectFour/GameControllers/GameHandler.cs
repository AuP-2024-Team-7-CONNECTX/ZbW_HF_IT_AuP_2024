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
		private readonly IUserRepository _UserRepository;
		private readonly IRobotRepository _robotRepository;
		private readonly IMoveRepository _moveRepository;
		private readonly IMqttService _mqttService;
		private readonly ILogger<GameHandler> _logger;

		private Game _game;
		private Move _currentMove;
		private Move? _lastMove;

		private Robot _robot1;
		private Robot _robot2;

		private User _User1;
		private User _User2;

		private static Random random = new Random();

		private bool RobotIsReadyForNextTurn = false;


		// 7 Horizontal, 6 vertikal Maximal
		private Dictionary<int, Dictionary<int, int>> _gameField;

		public GameHandler(IGameRepository gameRepository, IUserRepository UserRepository, IRobotRepository robotRepository, Game game, ILogger<GameHandler> logger, IMoveRepository moveRepository, IMqttService mqttService)
		{
			_gameRepository = gameRepository;
			_UserRepository = UserRepository;
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
			SetUpUsersAndRobots();
			ConnectWithMqtt();
			ChangeIsIngameState(true, GameState.InProgress);

			// First turn is decided randomly
			var beginner = random.Next(0, 2);

			if (beginner == 0)
			{
				_currentMove.Robot = _robot1;
				_currentMove.User = _User1;
			}
			else
			{
				_currentMove.Robot = _robot2;
				_currentMove.User = _User2;
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

				//if (!_currentMove.Robot.ControlledByHuman)
				//{

				//	// Keine Änderung passiert -> Algorithmus bestimmt

				//	if (_gameField == _game._gameField)
				//	{
				//		// PlaceNewStoneFromAlgorithm()

				//		// Spielfeld von Game aktualisieren, dass frontend informiert ist.
				//		_game.GameField = _gameField;

				//	}
				//}


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
			var UserNumber = _currentMove.User == _User1 ? 1 : 2;
			// Finde die erste leere Zeile in der angegebenen Spalte

			// hier column zuweisen aufgrund errechneter Wert mit Algorithmus -> muss noch implementiert werden

			for (int zeile = 0; zeile < _gameField[column].Count; zeile++)
			{
				if (_gameField[column][zeile] == 0)
				{
					// Setze den Stein des Spielers in das Feld
					_gameField[column][zeile] = UserNumber;

					return;
				}
			}
			throw new Exception($"Spalte {column} ist voll.");
		}


		private void SetUpUsersAndRobots()
		{
			_robot1 = _game.Robots[0];
			_robot2 = _game.Robots[1];
			if (_robot1.CurrentUser != null)
			{
				_User1 = _robot1.CurrentUser;
				_robot1.ControlledByHuman = true;
			}
			else
			{
				_User1 = null;
				_robot1.ControlledByHuman = false;
			}

			if (_robot2.CurrentUser != null)
			{
				_User2 = _robot2.CurrentUser;
				_robot2.ControlledByHuman = true;
			}
			else
			{
				_User2 = null;
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
			if (_currentMove.User == _User1 && _currentMove.Robot == _robot1)
			{
				_currentMove.Robot = _robot2;
				_currentMove.User = _User2;
			}
			else
			{
				_currentMove.Robot = _robot1;
				_currentMove.User = _User1;
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
			// needed ?
			//await _mqttService.ConnectToNewBrokerAsync();
			//await _mqttService.SubscribeAsync($"{_robot1.Topic}/feedback");
			//await _mqttService.SubscribeAsync($"{_robot2.Topic}/feedback");
			await _mqttService.RegisterGameHandler(this);
		}


		private async void DisconnectFromMqtt()
		{
			// needed?
			//await _mqttService.UnsubscribeAsync($"{_robot1.Topic}/feedback");
			//await _mqttService.UnsubscribeAsync($"{_robot2.Topic}/feedback");
			await _mqttService.DisconnectAsync();
		}


		private void ChangeIsIngameState(bool isIngame, GameState gameState)
		{
			try
			{
				foreach (var robot in _game.Robots)
				{
					robot.IsIngame = isIngame;
					var currentUser = robot.CurrentUser;
					currentUser.IsIngame = isIngame;
					_robotRepository.CreateOrUpdateAsync(robot);
					_UserRepository.CreateOrUpdateAsync(currentUser);
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
