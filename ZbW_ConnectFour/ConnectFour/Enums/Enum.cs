namespace ConnectFour.Enums
{
    public class Enum
    {
        public enum GameState
        {
            Completed,
            Aborted,
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
