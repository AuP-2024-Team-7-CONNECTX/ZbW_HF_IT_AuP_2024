using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GameController : ControllerBase
	{
		private readonly IGameRepository _gameRepository;
		private readonly IPlayerRepository _playerRepository;

		private readonly IRobotRepository _robotRepository;
		private readonly ILogger<GameController> _logger;


		public GameController(IGameRepository gameRepository, IPlayerRepository playerRepository, IRobotRepository robotRepository, ILogger<GameController> logger)
		{
			_gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_playerRepository = playerRepository;
			_robotRepository = robotRepository;
		}

		// GET: api/games
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Game>>> GetAll()
		{
			try
			{
				var games = await _gameRepository.GetAllAsync();
				return Ok(games);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while fetching all games.{ex.Message}");
				return StatusCode(500, $"An internal server error has occurred.{ex.Message}");
			}
		}

		// GET: api/games/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Game>> Get(string id)
		{
			try
			{
				var game = await _gameRepository.GetByIdAsync(id);
				if (game == null)
				{
					return NotFound("Game not found.");
				}
				return Ok(game);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while fetching the game with ID {id}.{ex.Message}");
				return StatusCode(500, $"An internal server error has occurred.{ex.Message}");
			}
		}

		// POST: api/games
		[HttpPost]
		public async Task<ActionResult<Game>> Post([FromBody] GameRequest request)
		{
			try
			{
				
				var allRobots = await _robotRepository.GetAllAsync();
				var listRobots = allRobots.ToList();
				var robots = listRobots.Where(r => request.RobotIds.Contains(r.Id)).ToList();

				if (robots.Count() != 2)
				{
					throw new Exception("Too many/few Robots in List. Couldnt create game");
				}

				var players = robots.Select(r => r.CurrentPlayer).ToList();

				if (players.Count() != 2)
				{
					throw new Exception("Too many/few Players in List. Couldnt create game");
				}

				var game = new Game
				{
					Id = Guid.NewGuid().ToString(),
					//Players = players,
					Robots = robots,
					CurrentMoveId = request.CurrentMoveId,
					State = GameState.InProgress,

					// On creating a Game, the Field is empty
					GameFieldJson = @"{ ""1"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""2"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""3"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""4"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""5"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""6"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0},
            ""7"": {""A"": 0, ""B"": 0, ""C"": 0, ""D"": 0, ""E"": 0, ""F"": 0}
        }"
				};

				await _gameRepository.CreateOrUpdateAsync(game);
				return CreatedAtAction(nameof(Get), new { id = game.Id }, game);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while creating a new game.{ex.Message}");
				return StatusCode(500, $"An internal server error has occurred. {ex.Message}");
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
					return NotFound("Game not found.");
				}

				if (!Enum.TryParse<GameState>(request.State, out var state))
				{
					return StatusCode(500, "Invalid value for Color");
				}

				game.State = state;
				game.CurrentMoveId = request.CurrentMoveId;
				game.GameFieldJson = request.GameFieldJson;

				await _gameRepository.CreateOrUpdateAsync(game);

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while updating the game with ID {id}.{ex.Message}");
				return StatusCode(500, $"An internal server error has occurred.{ex.Message}");
			}
		}

		// DELETE: api/games/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				var game = await _gameRepository.GetByIdAsync(id);
				if (game == null)
				{
					return NotFound("Game not found.");
				}

				await _gameRepository.DeleteAsync<Game>(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while deleting the game with ID {id}.{ex.Message}");
				return StatusCode(500, $"An internal server error has occurred.{ex.Message}");
			}
		}
	}
}
