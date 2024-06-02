namespace ConnectFour.Enums
{
    public class Enum
    {
        public enum GameState
        {
            Completed,
            Abandoned,
            InProgress
        }

        public enum ConnectFourColor { Red, Blue }

		public enum GameMode
		{
			PlayerVsPlayer,
			PlayerVsRobot,
			RobotVsRobot
		}

	}
}
