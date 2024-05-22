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
			Response.Headers.Add("Access-Control-Allow-Origin", "*");
			Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");
			Response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");


			return Ok();
		}

		[HttpOptions]
		public IActionResult Options()
		{
			// CORS-Header werden jetzt global gesetzt, keine Notwendigkeit, sie hier hinzuzufügen
			return Ok();
		}
	}
}
