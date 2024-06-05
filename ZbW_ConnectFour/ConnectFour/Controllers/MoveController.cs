﻿using ConnectFour.GameControllers;
using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
	[Route("Move")]
	[ApiController]
	public class MoveController : ControllerBase
	{
		private readonly IMoveRepository _moveRepository;
		private readonly ILogger<MoveController> _logger;
		private readonly IGameRepository _gameRepository;
		private readonly IUserRepository _userRepository;
		private readonly IRobotRepository _robotRepository;
		private readonly IGameHandlerService _gameHandlerService;

		private JsonResponseMessage _responseJson;

		public MoveController(IMoveRepository moveRepository, ILogger<MoveController> logger, IGameRepository gameRepository, IUserRepository userRepository, IRobotRepository robotRepository, IGameHandlerService gameHandlerService)
		{
			_moveRepository = moveRepository ?? throw new ArgumentNullException(nameof(moveRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_gameRepository = gameRepository;
			_userRepository = userRepository;
			_robotRepository = robotRepository;

			_responseJson = new JsonResponseMessage();
			_gameHandlerService = gameHandlerService;
		}

		// GET api/Moves
		[HttpGet]
		public async Task<ActionResult<IEnumerable<MoveResponse>>> GetAll()
		{
			try
			{
				var moves = await _moveRepository.GetAllAsync();
				var moveResponses = moves.Select(move => new MoveResponse
				{
					Id = move.Id,
					PlayerId = move.PlayerId,
					RobotId = move.RobotId,
					MoveDetails = move.MoveDetails,
					Duration = move.Duration,
					GameId = move.GameId
				}).ToList();

				return Ok(moveResponses);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ein Fehler ist aufgetreten beim Abrufen aller Züge.");
				_responseJson.Message = $"{ex.Message}";
				return StatusCode(500, _responseJson);
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
					_responseJson.Message = $"Zug mit ID {id} nicht gefunden.";
					return StatusCode(400, _responseJson);
				}

				var moveResponse = new MoveResponse
				{
					Id = move.Id,
					PlayerId = move.PlayerId,
					RobotId = move.RobotId,
					MoveDetails = move.MoveDetails,
					Duration = move.Duration,
					GameId = move.GameId
				};

				return Ok(moveResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Abrufen des Zuges mit ID {id}. {ex.Message}");
				_responseJson.Message = $"{ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		// POST api/Moves
		[HttpPost]
		public async Task<ActionResult<Move>> Post([FromBody] MoveRequest moveRequest)
		{
			try
			{
				var robot = await _robotRepository.GetByIdAsync(moveRequest.RobotId);

				if (robot == null)
				{
					_responseJson.Message = $"Kein Roboter mit ID {moveRequest.RobotId} gefunden.";
					return StatusCode(404, _responseJson);
				}

				var game = await _gameRepository.GetByIdAsync(moveRequest.GameId);

				if (game == null)
				{
					_responseJson.Message = $"Kein Spiel mit ID {moveRequest.GameId} gefunden.";
					return StatusCode(404, _responseJson);
				}

				var move = new Move()
				{
					Id = Guid.NewGuid().ToString(),
					Robot = robot,
					Game = game,
					User = robot!.CurrentUser,
					Duration = moveRequest.Duration,
					MoveDetails = moveRequest.MoveDetails
				};

				await _moveRepository.CreateOrUpdateAsync(move);
				game.CurrentMove = move;
				await _gameRepository.CreateOrUpdateAsync(game);

				var gameHandler = _gameHandlerService.GetGameHandlerById(game.Id);
				gameHandler.ReceiveInput(move.MoveDetails, true);

				return CreatedAtAction(nameof(Get), new { id = move.Id }, move);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Erstellen eines neuen Zuges. {ex.Message}");
				_responseJson.Message = $"{ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		// PUT api/Moves/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> Put(string id, [FromBody] MoveRequest moveUpdate)
		{
			try
			{
				var move = await _moveRepository.GetByIdAsync(id);
				if (move == null)
				{
					_responseJson.Message = $"Zug mit ID {id} nicht gefunden.";
					return StatusCode(400, _responseJson);
				}

				var robot = await _robotRepository.GetByIdAsync(moveUpdate.RobotId);
				var game = await _gameRepository.GetByIdAsync(moveUpdate.GameId);

				if (robot == null)
				{
					_responseJson.Message = $"Kein Roboter mit ID {moveUpdate.RobotId} gefunden.";
					return StatusCode(404, _responseJson);
				}

				if (game == null)
				{
					_responseJson.Message = $"Kein Spiel mit ID {moveUpdate.GameId} gefunden.";
					return StatusCode(404, _responseJson);
				}

				move.Robot = robot;
				move.Game = game;
				move.User = robot.CurrentUser;
				move.MoveDetails = moveUpdate.MoveDetails ?? move.MoveDetails;

				await _moveRepository.CreateOrUpdateAsync(move);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Aktualisieren des Zuges mit ID {id}. {ex.Message}");
				_responseJson.Message = $"{ex.Message}";
				return StatusCode(500, _responseJson);
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
					_responseJson.Message = $"Zug mit ID {id} nicht gefunden.";
					return StatusCode(400, _responseJson);
				}

				await _moveRepository.DeleteAsync<Move>(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ein Fehler ist aufgetreten beim Löschen des Zuges mit ID {id}. {ex.Message}");
				_responseJson.Message = $"{ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}
	}
}
