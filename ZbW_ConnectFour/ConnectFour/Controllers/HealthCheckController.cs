using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectFour.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            var report = await _healthCheckService.CheckHealthAsync();

            // Erstelle ein Ergebnis basierend auf den Einträgen des Health Checks
            var results = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status == HealthStatus.Healthy
            }).ToList();

            var response = new
            {
                checks = results,
                overallStatus = report.Status == HealthStatus.Healthy
            };

            // Überprüfe den Gesamtstatus und setze den HTTP Status Code entsprechend
            return report.Status == HealthStatus.Healthy ? Ok(response) : StatusCode(503, response);
        }
    }
}
