using Swashbuckle.AspNetCore.Filters;
using static ConnectFour.Enums.Enum;
using System.Collections.Generic;

public class GameRequestExample : IExamplesProvider<GameRequest>
{
    public GameRequest GetExamples()
    {
        return new GameRequest
        {
            RobotIds = ["d99e778f-b820-4700-b560-73af320333f5", "d609c9bb-2cff-401d-9c9c-e964786e34c5"],
			CurrentMoveId = Guid.NewGuid().ToString(),
            State = "Active"
        };
    }
}
