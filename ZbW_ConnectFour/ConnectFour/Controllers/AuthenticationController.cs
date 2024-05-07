using Microsoft.AspNetCore.Mvc;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using System.Threading.Tasks;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILogger<AuthenticationController> logger;

		public AuthenticationController(ILogger<AuthenticationController> logger)
		{
			this.logger = logger;
		}

		
	}
}
