using FutureNHS.Api.Attributes;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        private readonly SharedSecrets _sharedSecrets;

        public ToolsController(ILogger<ToolsController> logger, IPermissionsService permissionsService, IOptionsSnapshot<SharedSecrets> sharedSecrets)
        {
            _logger = logger;
            _permissionsService = permissionsService;
            _sharedSecrets = sharedSecrets.Value;
        }


        [HttpPost]
        [HttpPut]
        [DisableFormValueModelBinding]
        [Route("viewrequestbody")]
        public async Task<IActionResult> ViewRequestBody(CancellationToken cancellationToken)
        {
            Request.EnableBuffering();

            Request.Body.Position = 0;

            var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();

            return Ok(rawRequestBody);
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [DisableFormValueModelBinding]
        [Route("viewrequestheaders")]
        public async Task<IActionResult> ViewRequestHeaders(CancellationToken cancellationToken)
        {
            var headers = Request.Headers;
            var list = headers.ToList();

            return Ok(list);
        }

        [AllowAnonymous]
        [Route("viewApiKey")]
        public async Task<IActionResult> ViewApikey(CancellationToken cancellationToken)
        {

            return Ok(_sharedSecrets);
        }
    }
}