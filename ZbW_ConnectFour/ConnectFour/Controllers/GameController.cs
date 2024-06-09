using ConnectFour.Api.User;
using ConnectFour.Models;
using ConnectFour.Mqtt;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GameController : ControllerBase
	{
		private readonly IGameRepository _gameRepository;
		private readonly IUserRepository _userRepository;

		private readonly IRobotRepository _robotRepository;
		private readonly ILogger<GameController> _logger;

		private readonly IMqttAndGameService _gameHandlerService;

		public GameController(IGameRepository gameRepository, IUserRepository userRepository, IRobotRepository robotRepository, ILogger<GameController> logger, IMqttAndGameService gameHandlerService)
		{
			_gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_userRepository = userRepository;
			_robotRepository = robotRepository;
			_gameHandlerService = gameHandlerService;
		}

		// GET: api/games
		[HttpGet]
		public async Task<ActionResult<IEnumerable<GameResponse>>> GetAll()
		{
			try
			{
				var games = await _gameRepository.GetAllAsync();
				var gameResponses = games.Select(game => new GameResponse
				{
					Id = game.Id,
					Users = game.Users.Select(u => new UserResponse(
						u.Id,
						u.Name,
						u.Email,
						u.Password,
						u.Authenticated,
						u.IsIngame
					)),
					Robots = game.Robots.Select(r => new RobotResponse
					{
						Id = r.Id,
						Name = r.Name,
						// Fill other properties as needed
					}),
					CurrentMoveId = game.CurrentMoveId,
					WinnerUser = game.WinnerUser != null ? new UserResponse(
						game.WinnerUser.Id,
						game.WinnerUser.Name,
						game.WinnerUser.Email,
						game.WinnerUser.Password,
						game.WinnerUser.Authenticated,
						game.WinnerUser.IsIngame
					) : null,
					WinnerRobot = game.WinnerRobot != null ? new RobotResponse
					{
						Id = game.WinnerRobot.Id,
						Name = game.WinnerRobot.Name,
						// Fill other properties as needed
					} : null,
					State = game.State,
					TotalPointsPlayerOne = game.TotalPointsUserOne,
					TotalPointsPlayerTwo = game.TotalPointsUserTwo,
					NewTurnForFrontend = game.NewTurnForFrontend,
					NewTurnForFrontendRowColumn = game.NewTurnForFrontendRowColumn,
					GameMode = game.GameMode.ToString(),
					ManualTurnIsAllowed = game.ManualTurnIsAllowed
				});

				return Ok(gameResponses);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Abrufen aller Spiele. {ex.Message}");
				return StatusCode(500, new { Message = $"Ein interner Serverfehler ist aufgetreten: {ex.Message}", success = false });
			}
		}

		// GET: api/games/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<GameResponse>> Get(string id)
		{
			try
			{


				var game = await _gameRepository.GetByIdAsync(id);
				if (game == null)
				{
					return Ok(new { Message = "Spiel nicht gefunden.", success = false });
				}

				var gameFromMqttService = _gameHandlerService.GetGameById(game.Id);
			
				var gameResponse = new GameResponse
				{
					Id = game.Id,
					Users = game.Users.Select(u => new UserResponse(
						u.Id,
						u.Name,
						u.Email,
						u.Password,
						u.Authenticated,
						u.IsIngame
					)),
					Robots = game.Robots.Select(r => new RobotResponse
					{
						Id = r.Id,
						Name = r.Name,
						// Fill other properties as needed
					}),
					CurrentMoveId = game.CurrentMoveId,
					WinnerUser = game.WinnerUser != null ? new UserResponse(
						game.WinnerUser.Id,
						game.WinnerUser.Name,
						game.WinnerUser.Email,
						game.WinnerUser.Password,
						game.WinnerUser.Authenticated,
						game.WinnerUser.IsIngame
					) : null,
					WinnerRobot = game.WinnerRobot != null ? new RobotResponse
					{
						Id = game.WinnerRobot.Id,
						Name = game.WinnerRobot.Name,
						// Fill other properties as needed
					} : null,
					State = game.State,
					TotalPointsPlayerOne = game.TotalPointsUserOne,
					TotalPointsPlayerTwo = game.TotalPointsUserTwo,
					NewTurnForFrontend = gameFromMqttService != null ? gameFromMqttService.NewTurnForFrontend : game.NewTurnForFrontend,
					NewTurnForFrontendRowColumn = gameFromMqttService != null ? gameFromMqttService.NewTurnForFrontendRowColumn : game.NewTurnForFrontendRowColumn,
					GameMode = game.GameMode.ToString(),
					ManualTurnIsAllowed = gameFromMqttService != null ? gameFromMqttService.ManualTurnIsAllowed : game.ManualTurnIsAllowed
				};

				return Ok(gameResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Abrufen des Spiels mit ID {id}. {ex.Message}");
				return StatusCode(500, new { Message = $"Ein interner Serverfehler ist aufgetreten: {ex.Message}", success = false });
			}
		}

		// POST: api/games
		[HttpPost]
		public async Task<ActionResult<Game>> Post([FromBody] GameRequest request)
		{
			try
			{
				var games = await _gameRepository.GetAllAsync();

				if (games.Any())
				{

					var gameInProgress = games.FirstOrDefault(g => g.State == GameState.InProgress);

					if (gameInProgress != null)
					{
						return CreatedAtAction(nameof(Get), new { id = gameInProgress.Id }, new { Message = "Spiel bereits durch anderen Benutzer erstellt", success = true, data = gameInProgress });
					}
				}


				var allRobots = await _robotRepository.GetAllAsync();
				var listRobots = allRobots.ToList();
				var robots = listRobots.Where(r => request.RobotIds.Contains(r.Id)).ToList();

				if (robots.Count() != 2)
				{
					throw new Exception("Zu viele/wenige Roboter in der Liste. Spiel konnte nicht erstellt werden.");
				}

				var Users = robots.Select(r => r.CurrentUser).ToList();

				if (request.GameMode != "UserVsUser" && Users.Count() != 2)
				{
					throw new Exception("Zu viele/wenige Benutzer in der Liste. Spiel konnte nicht erstellt werden.");
				}

				var gameField = new GameField();

				var options = new JsonSerializerOptions
				{
					WriteIndented = true
				};
				var gameFieldJson = JsonSerializer.Serialize(gameField, options);

				var random = new Random();

				var startingUserId = random.Next(2) == 0 ? Users[0].Id : Users[1].Id;

				var game = new Game
				{
					Id = Guid.NewGuid().ToString(),
					Users = Users,
					Robots = robots,
					CurrentMoveId = request.CurrentMoveId,
					State = GameState.InProgress,
					GameFieldJson = gameFieldJson,
					StartingUserId = startingUserId,
					NewTurnForFrontend = false,
					NewTurnForFrontendRowColumn = null,
					ManualTurnIsAllowed = true
				};

				game = await _gameHandlerService.CreateNewGame(game);
				game = await _gameHandlerService.StartGame(game);

				await _gameRepository.CreateOrUpdateAsync(game);
				return CreatedAtAction(nameof(Get), new { id = game.Id }, new { Message = "Spiel erfolgreich erstellt", success = true, data = game });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Erstellen eines neuen Spiels. {ex.Message}");
				return StatusCode(500, new { Message = $"Ein interner Serverfehler ist aufgetreten: {ex.Message}", success = false });
			}
		}

		// PUT: api/games/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, [FromBody] GameRequest request)
		{
			try
			{
				var game = await _gameRepository.GetByIdAsync(id);
				if (game == null)
				{
					return Ok(new { Message = "Spiel nicht gefunden.", success = false });
				}

				if (!Enum.TryParse<GameState>(request.State, out var state))
				{
					return Ok(new { Message = "Ungültiger Wert für Status", success = false });
				}

				game.State = state;
				if (game.State == GameState.Aborted)
				{
					await _gameHandlerService.AbortGame(game);
				}

				if (game.State == GameState.Completed)
				{
					await _gameHandlerService.EndGame(game);
				}

				game.CurrentMoveId = request.CurrentMoveId;

				await _gameRepository.CreateOrUpdateAsync(game);


				return Ok(new { Message = "Spiel erfolgreich aktualisiert", success = true });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Aktualisieren des Spiels mit ID {id}. {ex.Message}");
				return StatusCode(500, new { Message = $"Ein interner Serverfehler ist aufgetreten: {ex.Message}", success = false });
			}
		}

	}
}
