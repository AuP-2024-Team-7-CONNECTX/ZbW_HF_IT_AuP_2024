using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ConnectFour.Enums.Enum;


namespace ConnectFour.Models
{
	public class Game : Entity
	{

		public virtual List<User>? Users { get; set; }
		public virtual List<Robot>? Robots { get; set; }

		public string User1Id { get; set; }

		public string StartingUserId { get; set; }

		public string? CurrentUserId { get; set; }

		public string User2Id { get; set; }

		public string Robot1Id { get; set; }
		public string Robot2Id { get; set; }

		public virtual User? WinnerUser { get; set; }
		public string? WinnerUserId { get; set; }
		public virtual Robot? WinnerRobot { get; set; }
		public string? WinnerRobotId { get; set; }
		public GameState State { get; set; }
		public decimal? TotalPointsUserOne { get; set; }
		public decimal? TotalPointsUserTwo { get; set; }

		public bool ManualTurnIsAllowed { get; set; }

		public GameMode GameMode { get; set; }

		public bool? NewTurnForFrontend { get; set; }
		public string? NewTurnForFrontendRowColumn { get; set; }
		public bool? RobotIsReadyForNextTurn { get; set; }
		public bool? ShowTurnOnBoard { get; set; }

		[NotMapped]
		public bool OverrideDbGameForGet { get; set; }

		[NotMapped]
		public bool TurnWithAlgorithm { get; set; }

		[NotMapped]
		public int TurnColumnFromKI { get; set; }
		[NotMapped]

		public bool SendFeedbackAfterPayloadReceiveAllowed { get; set; }
		[NotMapped]

		public bool LastTurnReady { get; set; }

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
		public List<int> Column1 { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0 };

		[JsonPropertyName("2")]
		public List<int> Column2 { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0 };

		[JsonPropertyName("3")]
		public List<int> Column3 { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0 };

		[JsonPropertyName("4")]
		public List<int> Column4 { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0 };

		[JsonPropertyName("5")]
		public List<int> Column5 { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0 };

		[JsonPropertyName("6")]
		public List<int> Column6 { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0 };

		[JsonPropertyName("7")]
		public List<int> Column7 { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0 };

		public Dictionary<int, List<int>> GetColumns()
		{
			return new Dictionary<int, List<int>>()
		{
			{ 1, Column1 },
			{ 2, Column2 },
			{ 3, Column3 },
			{ 4, Column4 },
			{ 5, Column5 },
			{ 6, Column6 },
			{ 7, Column7 }
		};
		}

		public void UpdateColumn(int columnNumber, int player)
		{
			var columns = GetColumns();
			if (columns.ContainsKey(columnNumber))
			{
				var column = columns[columnNumber];
				for (int i = 0; i < column.Count; i++)
				{
					if (column[i] == 0)
					{
						column[i] = player;
						break;
					}
				}
			}
		}
	}

}
