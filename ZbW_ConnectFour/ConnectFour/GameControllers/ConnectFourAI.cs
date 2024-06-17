using ConnectFour.Models;
using System.Collections.Generic;
using System.Linq;

public class ConnectFourAI
{
	private const int MaxDepth = 5;

	public int GetBestMove(GameField gameField, int player)
	{
		int bestMove = -1;
		int bestValue = int.MinValue;

		var columns = gameField.GetColumns();

		foreach (var column in columns.Keys)
		{
			if (columns[column].Contains(0))
			{
				var newGameField = CloneGameField(gameField);
				newGameField.UpdateColumn(column, player);

				int moveValue = Minimax(newGameField, MaxDepth, false, player);

				if (moveValue > bestValue)
				{
					bestMove = column;
					bestValue = moveValue;
				}
			}
		}

		return bestMove;
	}

	private int Minimax(GameField gameField, int depth, bool isMaximizing, int player)
	{
		int opponent = player == 1 ? 2 : 1;
		int score = Evaluate(gameField, player, opponent);

		if (Math.Abs(score) == 1000 || depth == 0 || IsFull(gameField))
			return score;

		if (isMaximizing)
		{
			int bestValue = int.MinValue;
			var columns = gameField.GetColumns();

			foreach (var column in columns.Keys)
			{
				if (columns[column].Contains(0))
				{
					var newGameField = CloneGameField(gameField);
					newGameField.UpdateColumn(column, player);

					int value = Minimax(newGameField, depth - 1, false, player);
					bestValue = Math.Max(bestValue, value);
				}
			}
			return bestValue;
		}
		else
		{
			int bestValue = int.MaxValue;
			var columns = gameField.GetColumns();

			foreach (var column in columns.Keys)
			{
				if (columns[column].Contains(0))
				{
					var newGameField = CloneGameField(gameField);
					newGameField.UpdateColumn(column, opponent);

					int value = Minimax(newGameField, depth - 1, true, player);
					bestValue = Math.Min(bestValue, value);
				}
			}
			return bestValue;
		}
	}

	private int Evaluate(GameField gameField, int player, int opponent)
	{
		int score = 0;

		if (CheckWin(gameField, player))
		{
			return 1000;
		}

		if (CheckWin(gameField, opponent))
		{
			return -1000;
		}

		// Additional evaluation logic can be added here
		// for example, favoring moves that lead to potential wins

		return score;
	}

	private bool IsFull(GameField gameField)
	{
		var columns = gameField.GetColumns();
		return columns.All(c => c.Value.All(cell => cell != 0));
	}

	public bool CheckWin(GameField gameField, int player)
	{
		var columns = gameField.GetColumns();
		int rows = columns[1].Count;

		// Convert columns to a 2D array for easier access
		int[,] board = new int[7, rows];
		for (int col = 1; col <= 7; col++)
		{
			for (int row = 0; row < rows; row++)
			{
				board[col - 1, row] = columns[col][row];
			}
		}

		// Check horizontal win
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < 4; col++)
			{
				if (board[col, row] == player && board[col + 1, row] == player && board[col + 2, row] == player && board[col + 3, row] == player)
				{
					return true;
				}
			}
		}

		// Check vertical win
		for (int col = 0; col < 7; col++)
		{
			for (int row = 0; row < 3; row++)
			{
				if (board[col, row] == player && board[col, row + 1] == player && board[col, row + 2] == player && board[col, row + 3] == player)
				{
					return true;
				}
			}
		}

		// Check diagonal (positive slope) win
		for (int col = 0; col < 4; col++)
		{
			for (int row = 0; row < 3; row++)
			{
				if (board[col, row] == player && board[col + 1, row + 1] == player && board[col + 2, row + 2] == player && board[col + 3, row + 3] == player)
				{
					return true;
				}
			}
		}

		// Check diagonal (negative slope) win
		for (int col = 0; col < 4; col++)
		{
			for (int row = 3; row < 6; row++)
			{
				if (board[col, row] == player && board[col + 1, row - 1] == player && board[col + 2, row - 2] == player && board[col + 3, row - 3] == player)
				{
					return true;
				}
			}
		}

		return false;
	}

	private GameField CloneGameField(GameField original)
	{
		var newGameField = new GameField
		{
			Column1 = new List<int>(original.Column1),
			Column2 = new List<int>(original.Column2),
			Column3 = new List<int>(original.Column3),
			Column4 = new List<int>(original.Column4),
			Column5 = new List<int>(original.Column5),
			Column6 = new List<int>(original.Column6),
			Column7 = new List<int>(original.Column7)
		};

		return newGameField;
	}

	public int GetMovesLeft(GameField gameField, int player)
	{
		var columns = gameField.GetColumns();
		int movesPlayed = columns.Values.Sum(column => column.Count(cell => cell == player));
		return 21 - movesPlayed;
	}
}
