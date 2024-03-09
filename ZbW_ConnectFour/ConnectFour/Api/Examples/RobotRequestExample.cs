using Swashbuckle.AspNetCore.Filters;
using static ConnectFour.Enums.Enum;

public class RobotRequestExample : IExamplesProvider<RobotRequest>
{
    public RobotRequest GetExamples()
    {
        return new RobotRequest
        {
            CurrentPlayerId = Guid.NewGuid().ToString(),
            IsConnected = true,
            Color = "Red"
        };
    }
}
