using Microsoft.AspNetCore.Mvc;
using FutureNHS.Infrastructure.Repositories.Read.Interfaces;

namespace FutureNHS.Api.Controllers
{
    [ApiController]
    public sealed class HealthCheckController : ControllerBase
    {
        public HealthCheckController()
        {
        }

        [HttpGet]
        [Route("health-check")]
        public IActionResult GetImageAsync(Guid id)
        {
            return Ok();
        }
    }
}