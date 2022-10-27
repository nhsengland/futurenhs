using FutureNHS.Api.Attributes;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class AdminGroupsController : ControllerIdentityBase
    {
        private readonly ILogger<AdminGroupsController> _logger;
        private readonly IAdminGroupService _adminGroupService;

        public AdminGroupsController(ILogger<ControllerIdentityBase> baseLogger, IUserService userService, ILogger<AdminGroupsController> logger,
            IAdminGroupService adminGroupService) : base(baseLogger, userService)
        {
            _logger = logger;
            _adminGroupService = adminGroupService;
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        [Route("admin/groups")]
        public async Task<IActionResult> CreateGroupAsync(CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }

            await _adminGroupService.CreateGroupAsync(identity.MembershipUserId, Request.Body, Request.ContentType, cancellationToken);
            return Ok();
        }

    }
}


