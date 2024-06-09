namespace ConnectFour.Enums
{
    public class Enum
    {
        public enum GameState
        {
            // 0 
            Completed,
            // 1
            Aborted,
            // 2
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
