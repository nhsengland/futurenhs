using FutureNHS.Api.Attributes;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.SystemPages)]
    public sealed class SystemPagesController : ControllerBase
    {
        private readonly ILogger<SystemPagesController> _logger;
        private readonly ISystemPageDataProvider _systemPageDataProvider;

        public SystemPagesController(ILogger<SystemPagesController> logger, ISystemPageDataProvider systemPageDataProvider)
        {
            _logger = logger;
            _systemPageDataProvider = systemPageDataProvider;
        }

        [HttpGet]
        [TypeFilter(typeof(ETagFilter))]
        [Route("page/{systemPageSlug}")]

        public async Task<IActionResult> GetSystemPagesAsync(string systemPageSlug, CancellationToken cancellationToken = default)
        {
            var systemPage = await _systemPageDataProvider.GetSystemPageAsync(systemPageSlug, cancellationToken);

            if (systemPage is null) return NotFound();

            return Ok(systemPage);
        }
    }
}