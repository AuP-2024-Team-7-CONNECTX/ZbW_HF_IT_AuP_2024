using ConnectFour.Models;

public class ConnectFourAI
{
	private const int MaxDepth = 8;

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

		// Check horizontal, vertical, and diagonal lines for a win or potential win

		// Implement the logic to evaluate the board state and return a score
		// Positive score for player win/advantage, negative score for opponent win/advantage
		// Example: return 1000 if player wins, -1000 if opponent wins, otherwise calculate potential scores

		return score;
	}

	private bool IsFull(GameField gameField)
	{
		var columns = gameField.GetColumns();
		return columns.All(c => c.Value.All(cell => cell != 0));
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
}
