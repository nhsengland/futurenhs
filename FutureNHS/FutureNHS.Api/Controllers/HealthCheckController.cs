using FutureNHS.Infrastructure.Repositories.Read.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [ApiController]
    public sealed class HealthCheckController : ControllerBase
    { 
        private readonly IHealthCheckDataProvider _healthCheckDataProvider;

        public HealthCheckController(IHealthCheckDataProvider healthCheckDataProvider)
        {
            _healthCheckDataProvider = healthCheckDataProvider;
        }

        [HttpGet]
        [Route("health-check")]
        public async Task<IActionResult> HeartBeat(CancellationToken cancellationToken)
        {
            var connectionSuccessful = await _healthCheckDataProvider.CheckDatabaseConnectionAsync(cancellationToken);
            
            if(connectionSuccessful)
                return Ok();

            return StatusCode(500);
        }
    }
}