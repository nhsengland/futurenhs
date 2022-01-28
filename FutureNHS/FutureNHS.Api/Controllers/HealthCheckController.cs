using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public sealed class HealthCheckController : ControllerBase
    { 
        private readonly IHealthCheckDataProvider _healthCheckDataProvider;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(IHealthCheckDataProvider healthCheckDataProvider, ILogger<HealthCheckController> logger)
        {
            _healthCheckDataProvider = healthCheckDataProvider;
            _logger = logger;
        }

        [HttpGet]
        [Route("health-check")]
        public async Task<IActionResult> HeartBeat(CancellationToken cancellationToken)
        {
            var connectionSuccessful = await _healthCheckDataProvider.CheckDatabaseConnectionAsync(cancellationToken);
            
            return connectionSuccessful ? Ok() : StatusCode(500);
        }
    }
}