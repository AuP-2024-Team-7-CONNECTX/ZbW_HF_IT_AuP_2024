using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ConnectFour.Enums.Enum;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using ConnectFour.GameControllers;

namespace ConnectFour.Models
{
	public class Game : Entity
	{
		[NotMapped]
		public GameHandler? GameHandler { get; set; }

		public virtual List<User>? Users { get; set; }
		public virtual List<Robot>? Robots { get; set; }

		public string User1Id { get; set; }

		[NotMapped]
		public string StartingUserId { get; set; }

		public string User2Id { get; set; }

		public string Robot1Id { get; set; }
		public string Robot2Id { get; set; }

		public virtual Move? CurrentMove { get; set; }
		public string? CurrentMoveId { get; set; }
		public virtual User? WinnerUser { get; set; }
		public string? WinnerUserId { get; set; }
		public virtual Robot? WinnerRobot { get; set; }
		public string? WinnerRobotId { get; set; }
		public GameState State { get; set; }
		public decimal? TotalPointsUserOne { get; set; }
		public decimal? TotalPointsUserTwo { get; set; }

		public bool ManualTurnIsAllowed { get; set; } = false;

		public GameMode GameMode { get;set; }

		[NotMapped]
		public bool? NewTurnForFrontend { get; init; }
		[NotMapped]
		public string? NewTurnForFrontendRowColumn { get; init; }

		// Spielfeld als JSON in der Datenbank speichern
		public string GameFieldJson
		{
			get
			{
				var options = new JsonSerializerOptions
				{
					WriteIndented = true
				};
				return JsonSerializer.Serialize(GameField, options);
			}
			set
			{
				var options = new JsonSerializerOptions();
				GameField = JsonSerializer.Deserialize<GameField>(value, options);
			}
		}

		// Privates Feld für das Spiel, wird aus Json deserialisiert
		[NotMapped]
		public GameField GameField { get; set; } = new GameField();

	}

	public class GameField
	{
		[JsonPropertyName("1")]
		public Dictionary<int, int> Column1 { get; set; } = new Dictionary<int, int>();

		[JsonPropertyName("2")]
		public Dictionary<int, int> Column2 { get; set; } = new Dictionary<int, int>();

		[JsonPropertyName("3")]
		public Dictionary<int, int> Column3 { get; set; } = new Dictionary<int, int>();

		[JsonPropertyName("4")]
		public Dictionary<int, int> Column4 { get; set; } = new Dictionary<int, int>();

		[JsonPropertyName("5")]
		public Dictionary<int, int> Column5 { get; set; } = new Dictionary<int, int>();

		[JsonPropertyName("6")]
		public Dictionary<int, int> Column6 { get; set; } = new Dictionary<int, int>();

		[JsonPropertyName("7")]
		public Dictionary<int, int> Column7 { get; set; } = new Dictionary<int, int>();
	}
}
