using ConnectFour.Models;
using Microsoft.Extensions.Configuration;
using System;

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
                new User ("User1","User 1","www.test.ch1","test1231",false),
                 new User ("User2","User 2","www.test.ch2","test1232",false),
            };

        var players = new List<Player>
            {
                new Player { Id = Guid.NewGuid().ToString(), Name = "Player 1", UserId = "User1", IsIngame = false },
                new Player { Id = Guid.NewGuid().ToString(), Name = "Player 2", UserId = "User2", IsIngame = true }
            };

        context.Users.AddRange(users);
        context.Players.AddRange(players);
        context.SaveChanges();
    }
}
