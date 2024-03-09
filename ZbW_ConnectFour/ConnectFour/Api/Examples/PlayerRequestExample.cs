using Swashbuckle.AspNetCore.Filters;
using ConnectFour.Models;
using System.ComponentModel.DataAnnotations;

public class PlayerRequestExample : IExamplesProvider<PlayerRequest>
{
    public PlayerRequest GetExamples()
    {
        return new PlayerRequest
        {
            Name = "John Player",
            UserId = Guid.NewGuid().ToString(),
            IsIngame = true
        };
    }
}
