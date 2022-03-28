using System.Security;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Discussion;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FutureNHS.Api.Services
{
    public class UserService : IUserService
    {
        private const string listMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/list";

        private readonly ILogger<UserService> _logger;
        private readonly IUserAdminDataProvider _userAdminDataProvider;
        private readonly IPermissionsService _permissionsService;

        public UserService(ILogger<UserService> logger, IPermissionsService permissionsService, IUserAdminDataProvider userAdminDataProvider)
        {
            _permissionsService = permissionsService;
            _userAdminDataProvider = userAdminDataProvider;
            _logger = logger;
        }

        public async Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid userId, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, listMembersRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateDiscussionAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            

            return await _userAdminDataProvider.GetMembersAsync(offset, limit, sort, cancellationToken);
        }
    }
}
