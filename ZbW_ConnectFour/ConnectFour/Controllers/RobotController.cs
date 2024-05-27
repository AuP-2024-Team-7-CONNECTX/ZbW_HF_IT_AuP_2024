using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static ConnectFour.Enums.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class RobotController : ControllerBase
	{
		private readonly IRobotRepository _repository;
		private readonly ILogger<RobotController> _logger;

		private JsonResponseMessage _responseJson;

		public RobotController(IRobotRepository repository, ILogger<RobotController> logger)
		{
			_repository = repository;
			_logger = logger;

			_responseJson = new JsonResponseMessage();
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
					CurrentUserId = robot.CurrentUserId,
					Name = robot.Name,
					IsConnected = robot.IsConnected,
					Color = robot.Color,
					IsIngame = robot.IsIngame,
					BrokerAddress = robot.BrokerAddress,
					BrokerPort = robot.BrokerPort,
				}).ToList();

				return Ok(robotResponses);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while fetching all robots. {ex.Message}");
				_responseJson.Message = $"An error occurred while processing your request. {ex.Message}";
				return StatusCode(500, _responseJson);
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
					_responseJson.Message = "Robot not found.";
					return NotFound(_responseJson);
				}

				var robotResponse = new RobotResponse
				{
					Id = robot.Id,
					CurrentUserId = robot.CurrentUserId,
					IsConnected = robot.IsConnected,
					Name = robot.Name,
					Color = robot.Color,
					IsIngame = robot.IsIngame,
					BrokerAddress = robot.BrokerAddress,
					BrokerPort = robot.BrokerPort,
				};

				return Ok(robotResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while fetching the robot with ID {id}. {ex.Message}");
				_responseJson.Message = $"An error occurred while processing your request. {ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		// POST api/Robots
		[HttpPost]
		public async Task<ActionResult<RobotResponse>> Post(RobotRequest robotRequest)
		{
			//if (!Enum.TryParse<ConnectFourColor>(robotRequest.Color, out var color))
			//{
			//	_responseJson.Message = "Invalid value for Color";
			//	return StatusCode(500, _responseJson);
			//}

			try
			{
				var newRobot = new Robot
				{
					Id = Guid.NewGuid().ToString(),
					CurrentUserId = robotRequest.CurrentUserId,
					Name = robotRequest.Name,
					IsConnected = robotRequest.IsConnected,
					Color = null,
					IsIngame = false, // Initial false, robots won't be ingame upon creation
					BrokerAddress = robotRequest.BrokerAddress,
					BrokerPort = robotRequest.BrokerPort,
				};

				await _repository.CreateOrUpdateAsync(newRobot);

				return CreatedAtAction(nameof(GetById), new { id = newRobot.Id }, new RobotResponse
				{
					Id = newRobot.Id,
					Name = newRobot.Name,
					CurrentUserId = newRobot.CurrentUserId,
					IsConnected = newRobot.IsConnected,
					Color = newRobot.Color,
					IsIngame = newRobot.IsIngame,
					BrokerAddress = newRobot.BrokerAddress,
					BrokerPort = newRobot.BrokerPort,
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while creating a new robot.");
				_responseJson.Message = $"An error occurred while processing your request. {ex.Message}";
				return StatusCode(500, _responseJson);
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
					_responseJson.Message = "Robot not found.";
					return NotFound(_responseJson);
				}

				robotToUpdate.CurrentUserId = robotRequest.CurrentUserId;
				robotToUpdate.IsConnected = robotRequest.IsConnected;
				robotToUpdate.IsIngame = robotRequest.IsIngame;

				if (!Enum.TryParse<ConnectFourColor>(robotRequest.Color, out var color))
				{
					_responseJson.Message = "Invalid value for Color";
					return StatusCode(500, _responseJson);
				}

				robotToUpdate.Color = color;
				robotToUpdate.Name = robotRequest.Name;

				await _repository.CreateOrUpdateAsync(robotToUpdate);

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while updating the robot with ID {id}. {ex.Message}");
				_responseJson.Message = $"An error occurred while processing your request. {ex.Message}";
				return StatusCode(500, _responseJson);
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
					_responseJson.Message = "Robot not found.";
					return NotFound(_responseJson);
				}

				await _repository.DeleteAsync<Robot>(robotToDelete.Id);

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while deleting the robot with ID {id}. {ex.Message}");
				_responseJson.Message = $"An error occurred while processing your request. {ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}
	}
}
