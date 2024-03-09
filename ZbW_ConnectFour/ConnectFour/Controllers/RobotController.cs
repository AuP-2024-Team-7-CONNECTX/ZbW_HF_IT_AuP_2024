using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Controllers
{
    [Route("api/Robots")]
    [ApiController]
    public class RobotController : ControllerBase
    {
        private readonly IRobotRepository _repository;
        private readonly ILogger<RobotController> _logger;

        public RobotController(IRobotRepository repository, ILogger<RobotController> logger)
        {
            ArgumentNullException.ThrowIfNull(repository, nameof(repository));
            _repository = repository;
            _logger = logger;
        }

        // GET: api/Robots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RobotResponse>>> GetAll()
        {
            try
            {
                var robots = await _repository.GetAllAsync();
                var robotResponses = robots.Select(robot => new RobotResponse
                {
                    Id = robot.Id,
                    CurrentPlayerId = robot.CurrentPlayerId,
                    IsConnected = robot.IsConnected,
                    Color = robot.Color,
                    IsIngame = robot.IsIngame,
                    GameIds = robot.Games?.Select(g => g.Id).ToList()
                }).ToList();

                return Ok(robotResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all robots.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET api/Robots/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RobotResponse>> GetById(string id)
        {
            try
            {
                var robot = await _repository.GetByIdAsync(id);
                if (robot == null)
                {
                    return NotFound("Robot not found.");
                }

                var robotResponse = new RobotResponse
                {
                    Id = robot.Id,
                    CurrentPlayerId = robot.CurrentPlayerId,
                    IsConnected = robot.IsConnected,
                    Color = robot.Color,
                    IsIngame = robot.IsIngame,
                    GameIds = robot.Games.Select(g => g.Id).ToList()
                };

                return Ok(robotResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the robot with ID {RobotId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/Robots
        [HttpPost]
        public async Task<ActionResult<RobotResponse>> Post(RobotRequest robotRequest)
        {
            if (!Enum.TryParse<ConnectFourColor>(robotRequest.Color, out var color))
            {
                return StatusCode(500, "Invalid value for Color");
            }

            try
            {
                var newRobot = new Robot
                {
                    Id = Guid.NewGuid().ToString(),
                    CurrentPlayerId = robotRequest.CurrentPlayerId,
                    IsConnected = robotRequest.IsConnected,
                    Color = color,
                    IsIngame = false, // Initial false, robots wont be ingame upon creation
                    Games = new List<Game>() // Initial empty list, assuming Robots are not directly associated with games upon creation
                };

                await _repository.CreateOrUpdateAsync(newRobot);

                return CreatedAtAction(nameof(GetById), new { id = newRobot.Id }, new RobotResponse
                {
                    Id = newRobot.Id,
                    CurrentPlayerId = newRobot.CurrentPlayerId,
                    IsConnected = newRobot.IsConnected,
                    Color = newRobot.Color,
                    IsIngame = newRobot.IsIngame,
                    GameIds = newRobot.Games.Select(g => g.Id).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new robot.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/Robots/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, RobotRequest robotRequest)
        {
            try
            {
                var robotToUpdate = await _repository.GetByIdAsync(id);
                if (robotToUpdate == null)
                {
                    return NotFound("Robot not found.");
                }

                robotToUpdate.CurrentPlayerId = robotRequest.CurrentPlayerId;
                robotToUpdate.IsConnected = robotRequest.IsConnected;
                robotToUpdate.Color = Enum.Parse<ConnectFourColor>(robotRequest.Color);
                // Assuming IsIngame is managed internally and not directly updated

                await _repository.CreateOrUpdateAsync(robotToUpdate);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the robot with ID {RobotId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE api/Robots/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var robotToDelete = await _repository.GetByIdAsync(id);
                if (robotToDelete == null)
                {
                    return NotFound("Robot not found.");
                }

                await _repository.DeleteAsync<Robot>(robotToDelete.Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the robot with ID {RobotId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
