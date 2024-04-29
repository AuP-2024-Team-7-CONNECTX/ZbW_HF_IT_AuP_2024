using ConnectFour.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.Json;
using static ConnectFour.Enums.Enum;
using Microsoft.Identity.Client;

namespace ConnectFour.Models
{
	public class Game : Entity
	{
		public virtual List<Player>? Players { get; set; }
		public virtual List<Robot>? Robots { get; set; }

		public string Player1Id { get; set; }
		public string Player2Id { get; set; }

		public string Robot1Id { get; set; }
		public string Robot2Id { get; set; }

		public virtual Move? CurrentMove { get; set; }
		public string? CurrentMoveId { get; set; }
		public virtual Player? WinnerPlayer { get; set; }
		public string? WinnerPlayerId { get; set; }
		public virtual Robot? WinnerRobot { get; set; }
		public string? WinnerRobotId { get; set; }
		public GameState State { get; set; }
		public decimal? TotalPointsPlayerOne { get; set; }
		public decimal? TotalPointsPlayerTwo { get; set; }

		public bool ManualTurnIsAllowed { get; set; } = false;


		// Spielfeld als JSON in der Datenbank speichern
		public string GameFieldJson
		{
			get
			{

				JsonSerializerOptions options = new JsonSerializerOptions
				{
					ReferenceHandler = ReferenceHandler.Preserve,
					WriteIndented = true
				};

				return JsonSerializer.Serialize(_gameField, options);
			}
			set
			{
				JsonSerializerOptions options = new JsonSerializerOptions
				{
					ReferenceHandler = ReferenceHandler.Preserve
				};

				_gameField = JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, int>>>(value, options);
			}
		}

		// Privates Feld für das Spiel, wird aus Json deserialisiert
		[NotMapped]
		public Dictionary<int, Dictionary<int, int>> _gameField = new Dictionary<int, Dictionary<int, int>>();

		// Spielfeld:

		// hier in der spielelogik wird statt koordination mit buchstaben mit int gearbeitet.
		// beim request verschicken und receiven an / vom Roboter wird das entsprechend gemappt

		//		{
		//  "1": { // Spalte 1
		//    "1": 1, // Feld A der Spalte 1, belegt von Spieler 1
		//    "2": 1, // Feld B der Spalte 1, belegt von Spieler 1
		//    "3": 2, // Feld C der Spalte 1, belegt von Spieler 2
		//    "4": 2, // Feld D der Spalte 1, belegt von Spieler 2
		//    "5": 0, // Feld E der Spalte 1, nicht belegt
		//    "6": 0  // Feld F der Spalte 1, nicht belegt
		//  },
		//  "2": { // Spalte 2
		//    "1": 1, // Feld A der Spalte 2, belegt von Spieler 1
		//    "2": 1, // Feld B der Spalte 2, belegt von Spieler 1
		//    "3": 2, // Feld C der Spalte 2, belegt von Spieler 2
		//    "4": 1, // Feld D der Spalte 2, belegt von Spieler 1
		//    "5": 0, // Feld E der Spalte 2, nicht belegt
		//    "6": 0  // Feld F der Spalte 2, nicht belegt
		//  },
		//  "3": { // Spalte 3
		//    "A": 1, // Feld A der Spalte 3, belegt von Spieler 1
		//    "B": 2, // Feld B der Spalte 3, belegt von Spieler 2
		//    "C": 1, // Feld C der Spalte 3, belegt von Spieler 1
		//    "D": 2, // Feld D der Spalte 3, belegt von Spieler 2
		//    "E": 0, // Feld E der Spalte 3, nicht belegt
		//    "F": 0  // Feld F der Spalte 3, nicht belegt
		//  },
		//  "4": { // Spalte 4
		//    "A": 1, // Feld A der Spalte 4, belegt von Spieler 1
		//    "B": 2, // Feld B der Spalte 4, belegt von Spieler 2
		//    "C": 2, // Feld C der Spalte 4, belegt von Spieler 2
		//    "D": 1, // Feld D der Spalte 4, belegt von Spieler 1
		//    "E": 0, // Feld E der Spalte 4, nicht belegt
		//    "F": 0  // Feld F der Spalte 4, nicht belegt
		//  },
		//  "5": { // Spalte 5
		//    "A": 1, // Feld A der Spalte 5, belegt von Spieler 1
		//    "B": 1, // Feld B der Spalte 5, belegt von Spieler 1
		//    "C": 2, // Feld C der Spalte 5, belegt von Spieler 2
		//    "D": 1, // Feld D der Spalte 5, belegt von Spieler 1
		//    "E": 0, // Feld E der Spalte 5, nicht belegt
		//    "F": 0  // Feld F der Spalte 5, nicht belegt
		//  },
		//  "6": { // Spalte 6
		//    "A": 1, // Feld A der Spalte 6, belegt von Spieler 1
		//    "B": 1, // Feld B der Spalte 6, belegt von Spieler 1
		//    "C": 1, // Feld C der Spalte 6, belegt von Spieler 1
		//    "D": 2, // Feld D der Spalte 6, belegt von Spieler 2
		//    "E": 0, // Feld E der Spalte 6, nicht belegt
		//    "F": 0  // Feld F der Spalte 6, nicht belegt
		//  },
		//  "7": { // Spalte 7
		//    "A": 1, // Feld A der Spalte 7, belegt von Spieler 1
		//    "B": 2, // Feld B der Spalte 7, belegt von Spieler 2
		//    "C": 2, // Feld C der Spalte 7, belegt von Spieler 2
		//    "D": 1, // Feld D der Spalte 7, belegt von Spieler 1
		//    "E": 0, // Feld E der Spalte 7, nicht belegt
		//    "F": 0  // Feld F der Spalte 7, nicht belegt
		//  }
		//}

	}
}
