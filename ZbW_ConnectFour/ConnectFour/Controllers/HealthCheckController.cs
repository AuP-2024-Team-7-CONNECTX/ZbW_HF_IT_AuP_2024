using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[EnableCors("AllowAll")] // Diese Zeile kann entfernt werden, da CORS jetzt global konfiguriert ist
	public class HealthCheckController : ControllerBase
	{
		private readonly HealthCheckService _healthCheckService;

		public HealthCheckController(HealthCheckService healthCheckService)
		{
			_healthCheckService = healthCheckService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			
			return Ok("ich habe keine nerven mehr");
		}

		
	}
}
