using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
	[Route("api/Players")]
	[ApiController]
	public class PlayerController : ControllerBase
	{
		private readonly IPlayerRepository _repository;
		private readonly IUserRepository _UserRepository;

		private readonly ILogger<PlayerController> _logger;

		public PlayerController(IPlayerRepository repository, IUserRepository userRepository, ILogger<PlayerController> logger)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_UserRepository = userRepository;
		}

		// GET: api/Players
		[HttpGet]
		public async Task<ActionResult<IEnumerable<PlayerResponse>>> GetAll()
		{
			try
			{
				var players = await _repository.GetAllAsync();
				var playerResponses = players.Select(player => new PlayerResponse(
				player.Id,
				player.Name,
				player.UserId,
				player.IsIngame
				)).ToList();
				return Ok(playerResponses);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while fetching all players.{ex.Message}");
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}

		// GET api/Players/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<PlayerResponse>> Get(string id)
		{
			try
			{
				var player = await _repository.GetByIdAsync(id);
				if (player == null)
				{
					return NotFound("Player not found.");
				}

				var playerResponse = new PlayerResponse(
				player.Id,
				player.Name,
				player.UserId,
				player.IsIngame);

				return Ok(playerResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while fetching the player with ID {id}.{ex.Message}");
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}

		// POST api/Players
		[HttpPost]
		public async Task<ActionResult<PlayerResponse>> Post([FromBody] PlayerRequest request)
		{
			try
			{
				var newPlayer = new Player
				{
					Id = Guid.NewGuid().ToString(),
					Name = request.Name,
					UserId = request.UserId,
					IsIngame = false // Beim Erstellen eines Spielers ist er nicht ingame

				};

				await _repository.CreateOrUpdateAsync(newPlayer);

				var playerResponse = new PlayerResponse(newPlayer.Id, newPlayer.Name, newPlayer.UserId, newPlayer.IsIngame);
				return CreatedAtAction(nameof(Get), new { id = newPlayer.Id }, playerResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while creating a new player.{ex.Message}");
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}

		// PUT api/Players/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> Put(string id, [FromBody] PlayerRequest request)
		{
			try
			{
				var existingPlayer = await _repository.GetByIdAsync(id);
				if (existingPlayer == null)
				{
					return NotFound("Player not found.");
				}

				existingPlayer.Name = request.Name;
				existingPlayer.UserId = request.UserId;
				existingPlayer.IsIngame = request.IsIngame;
				// Das Aktualisieren der Spieleliste hängt von der Geschäftslogik ab und ist hier nicht dargestellt

				await _repository.CreateOrUpdateAsync(existingPlayer);

				return NoContent(); // 204 No Content als Antwort auf eine erfolgreiche Aktualisierung
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while updating the player with ID {id}.{ex.Message}");
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}


		// DELETE api/Players/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(string id)
		{
			try
			{
				await _repository.DeleteAsync<User>(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while deleting the player with ID {id}.{ex.Message}");
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}
	}
}
