using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
    [Route("api/Games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameRepository gameRepository, ILogger<GameController> logger)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameResponse>>> GetAll()
        {
            try
            {
                var games = await _gameRepository.GetAllAsync();
                var gameResponses = games.Select(game => new GameResponse
                {
                    Id = game.Id,
                    Players = game.Players?.Select(p => new PlayerResponse(p.Id, p.Name, p.UserId, p.IsIngame, p.Games?.Select(g => g.Id).ToList())).ToList(),
                    Robots = game.Robots?.Select(r => new RobotResponse
                    {
                        Id = r.Id,
                        CurrentPlayerId = r.CurrentPlayerId,
                        IsConnected = r.IsConnected,
                        Color = r.Color,
                        IsIngame = r.IsIngame,
                        GameIds = r.Games?.Select(g => g.Id).ToList()
                    }).ToList(),
                    CurrentMoveId = game.CurrentMoveId,
                    WinnerPlayer = game.WinnerPlayer != null ? new PlayerResponse(game.WinnerPlayer.Id, game.WinnerPlayer.Name, game.WinnerPlayer.UserId, game.WinnerPlayer.IsIngame, game.WinnerPlayer.Games?.Select(g => g.Id).ToList()) : null,
                    WinnerRobot = game.WinnerRobot != null ? new RobotResponse
                    {
                        Id = game.WinnerRobot.Id,
                        CurrentPlayerId = game.WinnerRobot.CurrentPlayerId,
                        IsConnected = game.WinnerRobot.IsConnected,
                        Color = game.WinnerRobot.Color,
                        IsIngame = game.WinnerRobot.IsIngame,
                        GameIds = game.WinnerRobot.Games?.Select(g => g.Id).ToList()
                    } : null,
                    State = game.State,
                    TotalPointsPlayerOne = game.TotalPointsPlayerOne,
                    TotalPointsPlayerTwo = game.TotalPointsPlayerTwo
                }).ToList();

                return Ok(gameResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all games.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        // GET api/Games/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GameResponse>> Get(string id)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(id);
                if (game == null)
                {
                    return NotFound("Game not found.");
                }

                var gameResponse = new GameResponse
                {
                    Id = game.Id,
                    Players = game.Players.Select(p => new PlayerResponse(p.Id, p.Name, p.UserId, p.IsIngame, p.Games?.Select(g => g.Id).ToList())).ToList(),
                    Robots = game.Robots.Select(r => new RobotResponse
                    {
                        Id = r.Id,
                        CurrentPlayerId = r.CurrentPlayerId,
                        IsConnected = r.IsConnected,
                        Color = r.Color,
                        IsIngame = r.IsIngame,
                        GameIds = r.Games?.Select(g => g.Id)
                    }).ToList(),
                    CurrentMoveId = game.CurrentMoveId,
                    WinnerPlayer = game.WinnerPlayer != null ? new PlayerResponse(game.WinnerPlayer.Id, game.WinnerPlayer.Name, game.WinnerPlayer.UserId, game.WinnerPlayer.IsIngame, game.WinnerPlayer.Games?.Select(g => g.Id).ToList()) : null,
                    WinnerRobot = game.WinnerRobot != null ? new RobotResponse
                    {
                        Id = game.WinnerRobot.Id,
                        CurrentPlayerId = game.WinnerRobot.CurrentPlayerId,
                        IsConnected = game.WinnerRobot.IsConnected,
                        Color = game.WinnerRobot.Color,
                        IsIngame = game.WinnerRobot.IsIngame,
                        GameIds = game.WinnerRobot.Games?.Select(g => g.Id)
                    } : null,
                    State = game.State,
                    TotalPointsPlayerOne = game.TotalPointsPlayerOne,
                    TotalPointsPlayerTwo = game.TotalPointsPlayerTwo
                };
                return Ok(gameResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the game with ID {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/Games
        [HttpPost]
        public async Task<ActionResult<GameResponse>> Post([FromBody] Game game)
        {
            try
            {
                // Hier sollte die Validierung der Eingabedaten erfolgen
                await _gameRepository.CreateOrUpdateAsync(game);
                // Hier sollte die GameResponse basierend auf dem neu erstellten Spiel erstellt werden
                return CreatedAtAction(nameof(Get), new { id = game.Id }, game); // Rückgabe der Basis Game-Daten ohne Umwandlung in GameResponse für das Beispiel
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new game.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/Games/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] Game gameUpdate)
        {
            try
            {
                var game = await _gameRepository.GetByIdAsync(id);
                if (game == null)
                {
                    return NotFound("Game not found.");
                }

                // Aktualisieren des Spiels mit den neuen Daten
                await _gameRepository.CreateOrUpdateAsync(gameUpdate); // Annahme: Die Methode aktualisiert das Spiel, wenn es existiert

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the game with ID {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/Games/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
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
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
