using FutureNHS.Api.DataAccess.Models.Identity;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{

    public class ControllerIdentityBase : Controller
    {
        private readonly ILogger<ControllerIdentityBase> _logger;
        private readonly IUserService _userService;


        public ControllerIdentityBase(ILogger<ControllerIdentityBase> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
      
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Identity> GetUserIdentityAsync(CancellationToken cancellationToken)
        {
            var userSub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userSub))
                throw new UnauthorizedAccessException();

            return await _userService.GetMemberIdentityAsync(userSub, cancellationToken);

        }
    }
}
