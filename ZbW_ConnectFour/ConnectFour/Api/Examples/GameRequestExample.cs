using Swashbuckle.AspNetCore.Filters;
using static ConnectFour.Enums.Enum;
using System.Collections.Generic;

public class GameRequestExample : IExamplesProvider<GameRequest>
{
    public GameRequest GetExamples()
    {
        return new GameRequest
        {
            CurrentMoveId = Guid.NewGuid().ToString(),
            State = "Active"
        };
    }
}
