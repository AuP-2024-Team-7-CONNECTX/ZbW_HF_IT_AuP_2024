using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GameRequestController : ControllerBase
	{
		private static ConcurrentDictionary<string, string> gameRequests = new ConcurrentDictionary<string, string>();

		[HttpPost("SendRequest")]
		public IActionResult SendRequest([FromBody] GameInvitationRequest request)
		{
			// Store the request in a thread-safe dictionary
			gameRequests[request.ReceiverId] = request.SenderId;

			return Ok(new { message = "Spielanfrage gesendet", success = true });
		}

		[HttpGet("CheckRequest/{receiverId}")]
		public IActionResult CheckRequest(string receiverId)
		{
			if (gameRequests.TryRemove(receiverId, out var senderId))
			{
				return Ok(new { senderId, success = true });
			}
			return Ok(new { success = false, message = "No game request found" });
		}
	}

	public class GameInvitationRequest
	{
		public string SenderId { get; set; }
		public string ReceiverId { get; set; }
	}
}
