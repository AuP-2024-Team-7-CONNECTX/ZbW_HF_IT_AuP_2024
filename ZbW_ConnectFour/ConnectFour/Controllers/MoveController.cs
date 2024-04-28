using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers;

[Route("api/Moves")]
[ApiController]
public class MoveController : ControllerBase
{
    private readonly IMoveRepository _moveRepository;
    private readonly ILogger<MoveController> _logger;

    public MoveController(IMoveRepository moveRepository, ILogger<MoveController> logger)
    {
        _moveRepository = moveRepository ?? throw new ArgumentNullException(nameof(moveRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET api/Moves
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Move>>> GetAll()
    {
        try
        {
            var moves = await _moveRepository.GetAllAsync();
            // Assuming MoveResponse is a DTO you'd like to use to shape your response
            var moveResponses = moves.Select(move => new MoveResponse
            {
                Id = move.Id,
                PlayerId = move.PlayerId,
                RobotId = move.RobotId,
                MoveDetails = move.MoveDetails,
                MoveStarted = move.MoveStarted,
                MoveFinished = move.MoveFinished,
                Duration = move.Duration,
                GameId = move.GameId
            }).ToList();

            return Ok(moveResponses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while fetching all moves.{ex.Message}");
            return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
        }
    }

    // GET api/Moves/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MoveResponse>> Get(string id)
    {
        try
        {
            var move = await _moveRepository.GetByIdAsync(id);
            if (move == null)
            {
                return NotFound("Move not found.");
            }

            var moveResponse = new MoveResponse
            {
                Id = move.Id,
                PlayerId = move.PlayerId,
                RobotId = move.RobotId,
                MoveDetails = move.MoveDetails,
                MoveStarted = move.MoveStarted,
                MoveFinished = move.MoveFinished,
                Duration = move.Duration,
                GameId = move.GameId
            };

            return Ok(moveResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while fetching the move with ID {id}.{ex.Message}");
            return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
        }
    }

    // POST api/Moves
    [HttpPost]
    public async Task<ActionResult<Move>> Post([FromBody] Move move)
    {
        try
        {
            move.MoveStarted = DateTime.Now;
            await _moveRepository.CreateOrUpdateAsync(move);
            // Assuming you want to return the created move as is
            return CreatedAtAction(nameof(Get), new { id = move.Id }, move);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while creating a new move.{ex.Message}");
            return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
        }
    }

    // PUT api/Moves/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(string id, [FromBody] Move moveUpdate)
    {
        try
        {
            var move = await _moveRepository.GetByIdAsync(id);
            if (move == null)
            {
                return NotFound("Move not found.");
            }

            moveUpdate.Id = move.Id; // Ensure the ID is set correctly
            await _moveRepository.CreateOrUpdateAsync(moveUpdate);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while updating the move with ID {id}.{ex.Message}");
            return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
        }
    }

    // DELETE api/Moves/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var move = await _moveRepository.GetByIdAsync(id);
            if (move == null)
            {
                return NotFound("Move not found.");
            }

            await _moveRepository.DeleteAsync<Move>(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while deleting the move with ID {id}.{ex.Message}");
            return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
        }
    }
}
