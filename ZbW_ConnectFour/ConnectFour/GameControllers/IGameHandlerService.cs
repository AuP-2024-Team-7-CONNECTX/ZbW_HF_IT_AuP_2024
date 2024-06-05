using ConnectFour.Models;
using System.Collections.Generic;

namespace ConnectFour.GameControllers
{
	public interface IGameHandlerService
	{
		GameHandler CreateNewGameHandler(Game game);
		GameHandler GetGameHandlerById(string gameId);
		bool RemoveGameHandler(string gameId);
		IEnumerable<GameHandler> GetAllGameHandlers();
	}
}
