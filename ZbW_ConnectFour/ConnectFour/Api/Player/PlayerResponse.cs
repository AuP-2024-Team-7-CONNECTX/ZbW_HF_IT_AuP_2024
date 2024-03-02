namespace ConnectFour.Models
{
    public record PlayerResponse
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string UserId { get; init; }

        public bool IsIngame { get; init; }

        // Hier könnten Sie entscheiden, ob und wie Sie Spiele darstellen wollen.
        // Zum Beispiel könnten Sie die IDs oder Namen der Spiele zurückgeben,
        // anstatt vollständige Game-Objekte zu verwenden, um zyklische Abhängigkeiten zu vermeiden.
        public List<string> GameIds { get; init; }

        public PlayerResponse(string id, string name, string userId, bool isIngame, List<string> gameIds)
        {
            Id = id;
            Name = name;
            UserId = userId;
            IsIngame = isIngame;
            GameIds = gameIds ?? new List<string>();
        }
    }
}
