using FutureNHS.Api.Attributes;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class AdminGroupsController : ControllerBase
    {
        private readonly ILogger<AdminGroupsController> _logger;
        private readonly IAdminGroupService _adminGroupService;

        public AdminGroupsController(ILogger<AdminGroupsController> logger,
            IAdminGroupService adminGroupService)
        {
            _logger = logger;
            _adminGroupService = adminGroupService;
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        [Route("users/{adminUserId:guid}/admin/groups")]
        public async Task<IActionResult> CreateGroupAsync(Guid adminUserId, CancellationToken cancellationToken)
        {
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }

            await _adminGroupService.CreateGroupAsync(adminUserId, Request.Body, Request.ContentType, cancellationToken);
            return Ok();
        }

    }
}


