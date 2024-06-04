using ConnectFour.Models;

namespace ConnectFour.GameControllers
{
    public interface IGameHandlerService
    {
        GameHandler CreateNewGameHandler(Game game);

    }
}
