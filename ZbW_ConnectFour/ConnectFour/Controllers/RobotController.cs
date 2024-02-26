using ConnectFour.Api.User;
using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
    [Route("api/Robots")]
    [ApiController]
    public class RobotController : ControllerBase
    {
        private readonly IRobotRepository _repository; // Stellen Sie sicher, dass ein passendes Repository existiert
        private readonly ILogger<RobotController> _logger;

        public RobotController(IRobotRepository repository, ILogger<RobotController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Implementieren Sie hier die Methoden ähnlich wie im PlayerController
        // Beispiel für eine GET-Methode:
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RobotResponse>>> GetAll()
        {
            try
            {
                var robots = await _repository.GetAllAsync();
                var robotResponses = robots.Select(robot => new RobotResponse(robot.Id, robot.CurrentPlayerId, robot.IsConnected, robot.Color, robot.IsIngame)).ToList();
                return Ok(robotResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all robots.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Implementieren Sie die POST, GET by ID, PUT und DELETE Methoden entsprechend
    }
}
