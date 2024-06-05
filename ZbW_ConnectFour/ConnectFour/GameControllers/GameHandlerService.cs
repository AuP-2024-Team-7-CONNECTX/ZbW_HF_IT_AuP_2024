using ConnectFour.Models;
using ConnectFour.Mqtt;
using ConnectFour.Repositories.Interfaces;
using System.Collections.Concurrent;

namespace ConnectFour.GameControllers
{
	public class GameHandlerService : IGameHandlerService
	{
		private readonly IGameRepository _gameRepository;
		private readonly IUserRepository _userRepository;
		private readonly IRobotRepository _robotRepository;
		private readonly IMoveRepository _moveRepository;
		private readonly IMqttService _mqttService;
		private readonly ILogger<GameHandler> _logger;
		private static readonly ConcurrentDictionary<string, GameHandler> _gameHandlers = new ConcurrentDictionary<string, GameHandler>();

		public GameHandlerService(IGameRepository gameRepository, IUserRepository userRepository, IRobotRepository robotRepository, IMoveRepository moveRepository, IMqttService mqttService, ILogger<GameHandler> logger)
		{
			_gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
			_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_robotRepository = robotRepository ?? throw new ArgumentNullException(nameof(robotRepository));
			_moveRepository = moveRepository ?? throw new ArgumentNullException(nameof(moveRepository));
			_mqttService = mqttService ?? throw new ArgumentNullException(nameof(mqttService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public GameHandler CreateNewGameHandler(Game game)
		{
			var gameHandler = new GameHandler(_gameRepository, _userRepository, _robotRepository, game, _logger, _moveRepository, _mqttService);
			_gameHandlers.TryAdd(game.Id, gameHandler);
			return gameHandler;
		}

		public GameHandler GetGameHandlerById(string gameId)
		{
			_gameHandlers.TryGetValue(gameId, out var gameHandler);
			return gameHandler;
		}

		public bool RemoveGameHandler(string gameId)
		{
			return _gameHandlers.TryRemove(gameId, out _);
		}

		public IEnumerable<GameHandler> GetAllGameHandlers()
		{
			return _gameHandlers.Values;
		}
	}
}
