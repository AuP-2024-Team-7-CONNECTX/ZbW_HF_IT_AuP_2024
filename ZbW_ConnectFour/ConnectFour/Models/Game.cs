﻿using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ConnectFour.Enums.Enum;
using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace ConnectFour.Models
{
	public class Game : Entity
	{
		public virtual List<User>? Users { get; set; }
		public virtual List<Robot>? Robots { get; set; }

		public string User1Id { get; set; }
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

		// Spielfeld:

		// hier in der spielelogik wird statt Koordination mit Buchstaben mit int gearbeitet.
		// beim Request verschicken und receiven an / vom Roboter wird das entsprechend gemappt

		// Beispiel für das Spielfeld:
		//  "1": { "1": 1, "2": 1, "3": 2, "4": 2, "5": 0, "6": 0 },
		//  "2": { "1": 1, "2": 1, "3": 2, "4": 1, "5": 0, "6": 0 },
		//  "3": { "1": 1, "2": 2, "3": 1, "4": 2, "5": 0, "6": 0 },
		//  "4": { "1": 1, "2": 2, "3": 2, "4": 1, "5": 0, "6": 0 },
		//  "5": { "1": 1, "2": 1, "3": 2, "4": 1, "5": 0, "6": 0 },
		//  "6": { "1": 1, "2": 1, "3": 1, "4": 2, "5": 0, "6": 0 },
		//  "7": { "1": 1, "2": 2, "3": 2, "4": 1, "5": 0, "6": 0 }
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
