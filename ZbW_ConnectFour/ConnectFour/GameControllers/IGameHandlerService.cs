using ConnectFour.Models;
using System.Threading.Tasks;

namespace ConnectFour.GameControllers
{
	public interface IGameHandlerService
	{
		Task<Game> CreateNewGame(Game game);
		Task<Game> UpdateGame(Game game);
		Task<Game> StartGame(Game game);
		Task<Game> EndGame(Game game);
		Task<Game> AbortGame(Game game);
		Task<Game> ReceiveInput(Game game, string payload, bool isFromFrontend);
		Task<bool> SendTurnToRobot(Game game, string payload);
		void PlaceNewStoneFromAlgorithm(Game game, int column);
	}
}
