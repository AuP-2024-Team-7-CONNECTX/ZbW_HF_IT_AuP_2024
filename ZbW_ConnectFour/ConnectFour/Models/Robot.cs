using ConnectFour.Interfaces;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Models
{
	public class Robot : Entity
	{
		public string Name { get; set; }

		public virtual Player? CurrentPlayer { get; set; }
		public string? CurrentPlayerId { get; set; }

		public bool IsConnected { get; set; }

		public ConnectFourColor Color { get; set; }

		public bool IsIngame { get; set; }

		
		public required string Endpoint { get; set; }

	}
}
