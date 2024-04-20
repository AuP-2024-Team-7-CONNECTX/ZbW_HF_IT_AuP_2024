using ConnectFour.Repositories.Interfaces;

namespace ConnectFour.GameControllers
{
	public class GameHandler
	{
		private readonly IGameRepository _gameRepository;
		private readonly IPlayerRepository _playerRepository;
		private readonly IRobotRepository _robotRepository;
		public GameHandler(IGameRepository gameRepository, IPlayerRepository playerRepository, IRobotRepository robotRepository)
		{
			_gameRepository = gameRepository;
			_playerRepository = playerRepository;
			_robotRepository = robotRepository;
		}

		public bool StartGame()
		{
			// set robot to unavailable

			// Update Player to ingame

			return true;
		}
	}
}
