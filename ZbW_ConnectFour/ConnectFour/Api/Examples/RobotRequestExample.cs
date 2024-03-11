using Swashbuckle.AspNetCore.Filters;
using static ConnectFour.Enums.Enum;

public class RobotRequestExample : IExamplesProvider<RobotRequest>
{
    public RobotRequest GetExamples()
    {
        return new RobotRequest
        {
            CurrentPlayerId = null,
            IsConnected = true,
            IsIngame = false,
            Color = "Red",
            Name ="Röbby"
        };
    }
}
