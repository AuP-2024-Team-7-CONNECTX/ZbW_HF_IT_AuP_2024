using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Controllers
{
	[Route("api/communication")]
	[ApiController]
	public class CommunicationController : ControllerBase
	{
		private readonly IGameRepository _gameRepository;
		private readonly IPlayerRepository _playerRepository;
			
		private readonly IRobotRepository _robotRepository;
		private readonly ILogger<CommunicationController> _logger;

		public CommunicationController(IGameRepository gameRepository, IPlayerRepository playerRepository, IRobotRepository robotRepository, ILogger<CommunicationController> logger)
		{
			_gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_playerRepository = playerRepository;
			_robotRepository = robotRepository;
		}




//		Roboterverwaltig:
//-/Subscribe(id)                     | Roboter sendet das
//-/SubscribeResponse					| Roboter empfängt das
//-/Unsubscribe(id)                   | Roboter sendet das
//-/RobotInitialized(id)                  | Roboter sendet das
//-/RobotIsInGame(id)                 | Roboter sendet das
//-/RobotGameIsDone(id)                   | Roboter sendet das

//		// dient dazu um zu zeigen, dass der Roboter bereit ist und in der spiellobby auswählbar ist als Gegner (oder halt im pvp als Spielsteinsetzer)

//Spielverwaltig:
//-/SendDuellRequest					| Roboter empfängt das
//-/DuellRequestAccepted(id)              | Roboter sendet das -> Er schickt auf allen URLs /RobotIsInGame
//-/Spielsteinsetzen(column, color)           | Robot sendet das.
//-/RecreateGameWhereConnectionLost(List<Spielzüege>) | Roboter empfängt das.
//-/SpielWasRecreated(id)                 | Robot empfängt das
//-/SpielzugWurdeAbgeschlossen oder einfach Ready 	| Roboter sendet das.
//-/Error(id)                     | Roboter sendet da			
//-/Abort							| Roboter empfängt das
	}
}
