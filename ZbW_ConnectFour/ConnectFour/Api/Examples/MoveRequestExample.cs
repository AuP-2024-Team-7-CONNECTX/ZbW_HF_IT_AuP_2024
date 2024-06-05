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
			Duration = 922.4f,	
			GameId = Guid.NewGuid().ToString()
		};
	}
}
