using System;

namespace ConnectFour
{
	public class Algorithm
	{
		private const int MaxDepth = 5; // Max. Tiefe für die Rekursion
		private const int Rows = 6;
		private const int Cols = 7;

		public int FindBestMove(int[,] board)
		{
			int bestMove = -1;
			int bestScore = int.MinValue;

			for (int col = 0; col < Cols; col++)
			{
				if (IsValidMove(board, col))
				{
					int[,] newBoard = MakeMove(board, col, 2); // 2 repräsentiert den Roboter

					int score = Minimax(newBoard, 0, false); // Starte Minimax

					if (score > bestScore)
					{
						bestScore = score;
						bestMove = col;
					}
				}
			}

			return bestMove;
		}

		private int Minimax(int[,] board, int depth, bool isMaximizingPlayer)
		{
			if (depth == MaxDepth || IsGameOver(board))
			{
				return EvaluateBoard(board);
			}

			if (isMaximizingPlayer)
			{
				int bestScore = int.MinValue;
				for (int col = 0; col < Cols; col++)
				{
					if (IsValidMove(board, col))
					{
						int[,] newBoard = MakeMove(board, col, 2);
						int score = Minimax(newBoard, depth + 1, false);
						bestScore = Math.Max(bestScore, score);
					}
				}
				return bestScore;
			}
			else
			{
				int bestScore = int.MaxValue;
				for (int col = 0; col < Cols; col++)
				{
					if (IsValidMove(board, col))
					{
						int[,] newBoard = MakeMove(board, col, 1); // 1 repräsentiert den Gegner
						int score = Minimax(newBoard, depth + 1, true);
						bestScore = Math.Min(bestScore, score);
					}
				}
				return bestScore;
			}
		}

		private bool IsValidMove(int[,] board, int col)
		{
			// Überprüfe, ob die oberste Reihe der Spalte leer ist
			return board[0, col] == 0;
		}

		private int[,] MakeMove(int[,] board, int col, int player)
		{
			// Führe den Zug für den Spieler aus
			int[,] newBoard = (int[,])board.Clone();

			for (int row = Rows - 1; row >= 0; row--)
			{
				if (newBoard[row, col] == 0)
				{
					newBoard[row, col] = player;
					break;
				}
			}

			return newBoard;
		}

		private bool IsGameOver(int[,] board)
		{
			// Implementiere Logik, um zu überprüfen, ob das Spiel vorbei ist
			return false;
		}

		private int EvaluateBoard(int[,] board)
		{
			// Implementiere eine Heuristik, um den Wert des Spielbretts zu bewerten
			return 0;
		}
	}
}
