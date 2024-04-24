using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

		// dient dazu um zu zeigen, dass der Roboter bereit ist und in der spiellobby auswählbar ist als Gegner (oder halt im pvp als Spielsteinsetzer)

		//Spielverwaltig:
		//-/SendDuellRequest					| Roboter empfängt das
		//-/DuellRequestAccepted(id)              | Roboter sendet das -> Er schickt auf allen URLs /RobotIsInGame
		//-/Spielsteinsetzen(column, color)           | Robot sendet das.
		//-/RecreateGameWhereConnectionLost(List<Spielzüege>) | Roboter empfängt das.
		//-/SpielWasRecreated(id)                 | Robot empfängt das
		//-/SpielzugWurdeAbgeschlossen oder einfach Ready 	| Roboter sendet das.
		//-/Error(id)                     | Roboter sendet da			
		//-/Abort							| Roboter empfängt das





		// Robot Management
		[HttpPost("Subscribe/{id}")]
		public IActionResult Subscribe(string id)
		{
			// Implement logic for the Subscribe command
			return Ok();
		}

		[HttpPost("SubscribeResponse")]
		public IActionResult SubscribeResponse()
		{
			// Implement logic for the SubscribeResponse command
			return Ok();
		}

		[HttpPost("Unsubscribe/{id}")]
		public IActionResult Unsubscribe(string id)
		{
			// Implement logic for the Unsubscribe command
			return Ok();
		}

		[HttpPost("RobotInitialized/{id}")]
		public IActionResult RobotInitialized(string id)
		{
			// Implement logic for the RobotInitialized command
			return Ok();
		}

		[HttpPost("RobotIsInGame/{id}")]
		public IActionResult RobotIsInGame(string id)
		{
			// Implement logic for the RobotIsInGame command
			return Ok();
		}

		[HttpPost("RobotGameIsDone/{id}")]
		public IActionResult RobotGameIsDone(string id)
		{
			// Implement logic for the RobotGameIsDone command
			return Ok();
		}

		// Game Management
		[HttpPost("SendDuellRequest")]
		public IActionResult SendDuellRequest()
		{
			// Implement logic for the SendDuellRequest command
			return Ok();
		}

		[HttpPost("DuellRequestAccepted/{id}")]
		public IActionResult DuellRequestAccepted(string id)
		{
			// Implement logic for the DuellRequestAccepted command
			return Ok();
		}

		[HttpPost("Spielsteinsetzen")]
		public IActionResult Spielsteinsetzen(int column, ConnectFourColor color)
		{
			// Implement logic for the Spielsteinsetzen command
			return Ok();
		}

		[HttpPost("RecreateGameWhereConnectionLost")]
		public IActionResult RecreateGameWhereConnectionLost(List<Move> spielzuege)
		{
			// Implement logic for the RecreateGameWhereConnectionLost command
			return Ok();
		}

		[HttpPost("SpielWasRecreated/{id}")]
		public IActionResult SpielWasRecreated(string id)
		{
			// Implement logic for the SpielWasRecreated command
			return Ok();
		}

		[HttpPost("SpielzugWurdeAbgeschlossen")]
		public IActionResult SpielzugWurdeAbgeschlossen()
		{
			// Implement logic for the SpielzugWurdeAbgeschlossen command
			return Ok();
		}

		[HttpPost("Ready")]
		public IActionResult Ready()
		{
			// Implement logic for the Ready command
			return Ok();
		}

		[HttpPost("Error/{id}")]
		public IActionResult Error(string id)
		{
			// Implement logic for the Error command
			return Ok();
		}

		[HttpPost("Abort")]
		public IActionResult Abort()
		{
			// Implement logic for the Abort command
			return Ok();
		}
	}
}
