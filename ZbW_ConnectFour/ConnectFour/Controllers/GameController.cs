using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;

        public GameController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

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
                _logger.LogError(ex, "An error occurred while fetching all games.");
                return StatusCode(500, "An internal server error has occurred.");
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
                _logger.LogError(ex, $"An error occurred while fetching the game with ID {id}.");
                return StatusCode(500, "An internal server error has occurred.");
            }
        }

        // POST: api/games
        [HttpPost]
        public async Task<ActionResult<Game>> Post([FromBody] GameRequest request)
        {
            try
            {
                //var players = await _playerRepository.GetAllAsync();
                //var playersGame = (List<Player>)players.Where(p => request.PlayerIds.Contains(p.Id));

                var allRobots = await _robotRepository.GetAllAsync();
                var robots = (List<Robot>)allRobots.Where(r => request.RobotIds.Contains(r.Id));

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
                    Players = players,
                    Robots = robots,
                    CurrentMoveId = request.CurrentMoveId,
                    State = GameState.Active
                };

                await _gameRepository.CreateOrUpdateAsync(game);
                return CreatedAtAction(nameof(Get), new { id = game.Id }, game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new game.");
                return StatusCode(500, "An internal server error has occurred.");
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
               
                await _gameRepository.CreateOrUpdateAsync(game);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the game with ID {id}.");
                return StatusCode(500, "An internal server error has occurred.");
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
                _logger.LogError(ex, $"An error occurred while deleting the game with ID {id}.");
                return StatusCode(500, "An internal server error has occurred.");
            }
        }
    }
}
