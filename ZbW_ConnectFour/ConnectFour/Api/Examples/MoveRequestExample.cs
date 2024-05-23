using Swashbuckle.AspNetCore.Filters;
using System;

public class MoveRequestExample : IExamplesProvider<MoveRequest>
{
	public MoveRequest GetExamples()
	{
		return new MoveRequest
		{
			RobotId = Guid.NewGuid().ToString(),
			MoveDetails = "Column 3",
			MoveStarted = DateTime.UtcNow,
			MoveFinished = DateTime.UtcNow.AddSeconds(5),
			GameId = Guid.NewGuid().ToString()
		};
	}
}
