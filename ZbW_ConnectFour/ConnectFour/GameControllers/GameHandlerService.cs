using ConnectFour.Models;
using ConnectFour.Mqtt;
using ConnectFour.Repositories.Interfaces;

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

		public GameHandlerService(IGameRepository gameRepository, IUserRepository userRepository, IRobotRepository robotRepository, IMoveRepository moveRepository, IMqttService mqttService)
		{
			_gameRepository = gameRepository;
			_userRepository = userRepository;
			_robotRepository = robotRepository;
			_moveRepository = moveRepository;
			_mqttService = mqttService;
		}

		public GameHandler CreateNewGameHandler(Game game)
        {
            return new GameHandler(_gameRepository,_userRepository,_robotRepository, game, _logger,_moveRepository,_mqttService);
        }
    }
}
