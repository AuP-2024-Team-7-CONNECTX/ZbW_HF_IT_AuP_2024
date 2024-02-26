using ConnectFour.Models;

public class PlayerResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }

    public bool IsIngame { get; set; }

    public List<Game>? Games { get; set; }

    public PlayerResponse(string id, string name, string userId, bool isIngame, List<Game> games)
    {
        Id = id;
        Name = name;
        UserId = userId;
        IsIngame = isIngame;
        Games = games;
    }
}