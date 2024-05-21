using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectFour.Controllers;

[ApiController]
[Route("[controller]")]
[EnableCors("AllowAll")]
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
		//// Setze CORS-Header in der Antwort
		Response.Headers.Add("Access-Control-Allow-Origin", "*"); // Erlaubt den Zugriff von allen Herkunftsorten
																  ////Response.Headers.Add("Access-Control-Allow-Methods", "GET"); // Erlaubt POST-Anfragen
																  //Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type"); // Erlaubt bestimmte Header
																  //Response.Headers.Add("Access-Control-Allow-Credentials", "true"); // Erlaubt bestimmte Header


		return Ok("test Text 123");
	}
}
