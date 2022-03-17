using FutureNHS.Api.Attributes;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    //[FeatureGate(FeatureFlags.Groups)]
    public sealed class ToolsController : ControllerBase
    {
        private readonly ILogger<ToolsController> _logger;
        private readonly IPermissionsService _permissionsService;

        public ToolsController(ILogger<ToolsController> logger, IPermissionsService permissionsService)
        {
            _logger = logger;
            _permissionsService = permissionsService;
        }


        [HttpPost]
        [DisableFormValueModelBinding]
        [Route("viewrequestbody")]
        public async Task<IActionResult> ViewRequestBody(CancellationToken cancellationToken)
        {
            Request.EnableBuffering();

            Request.Body.Position = 0;

            var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();

            return Ok(rawRequestBody);
        }
    }
}