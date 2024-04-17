using ConnectFour.Models;
using Microsoft.Extensions.Configuration;

public class TestConfigurationHelper
{
    public static IConfigurationRoot Configuration { get; set; }

    static TestConfigurationHelper()
    {
        BuildConfiguration();
    }

    private static void BuildConfiguration()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static string GetConnectionString()
    {
        return Configuration.GetConnectionString("DefaultConnection");
    }

    public static void InitializeDbForTests(GameDbContext context)
    {
        var users = new List<User>
            {
                new User {Id = Guid.NewGuid().ToString(),Name = "User 1",Email="www.test.ch1",Authenticated=false,Password="Hash123" },
                new User {Id = Guid.NewGuid().ToString(),Name = "User 2",Email="www.test.ch2",Authenticated=false,Password="Hash123",DeletedOn=DateTime.Now },
                new User {Id = Guid.NewGuid().ToString(),Name = "User 3",Email="www.test.ch3",Authenticated=true,Password="Hash123" },

            };

        var players = new List<Player>
            {
                new Player { Id = Guid.NewGuid().ToString(),User = users.FirstOrDefault(d => d.Name=="User 1"), Name = "Player 1", UserId = "User1", IsIngame = false },
                new Player { Id = Guid.NewGuid().ToString(), User = users.FirstOrDefault(d => d.Name=="User 2"), Name = "Player 2", UserId = "User2", IsIngame = true },
                new Player { Id = Guid.NewGuid().ToString(), User = users.FirstOrDefault(d => d.Name=="User 3"),  Name = "Player 3", UserId = "User3", IsIngame = false,DeletedOn=DateTime.Now }

            };

        var robots = new List<Robot>
            {
                new Robot{ Id = Guid.NewGuid().ToString(),CurrentPlayerId = players.FirstOrDefault(d => d.Name=="Player 1").Id,Color=ConnectFour.Enums.Enum.ConnectFourColor.Red,IsConnected=false,IsIngame=false,CurrentPlayer=null,Name="Robot1"},
            };

        context.Users.AddRange(users);
        context.Players.AddRange(players);
        context.Robots.AddRange(robots);
        context.SaveChanges();
    }
}
