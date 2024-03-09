using Swashbuckle.AspNetCore.Filters;
using static ConnectFour.Enums.Enum;
using System.Collections.Generic;

public class GameRequestExample : IExamplesProvider<GameRequest>
{
    public GameRequest GetExamples()
    {
        return new GameRequest
        {
            PlayerIds = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
            RobotIds = new List<string> { Guid.NewGuid().ToString() },
            CurrentMoveId = Guid.NewGuid().ToString(),
            WinnerPlayerId = Guid.NewGuid().ToString(),
            State = GameState.InProgress
        };
    }
}
