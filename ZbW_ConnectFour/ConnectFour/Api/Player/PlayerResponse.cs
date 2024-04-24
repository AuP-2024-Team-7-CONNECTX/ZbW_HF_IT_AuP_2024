namespace ConnectFour.Models
{
	public record PlayerResponse
	{
		public string Id { get; init; }

		public string Name { get; init; }

		public string UserId { get; init; }

		public bool IsIngame { get; init; }


		public PlayerResponse(string id, string name, string userId, bool isIngame)
		{
			Id = id;
			Name = name;
			UserId = userId;
			IsIngame = isIngame;
		}
	}
}
