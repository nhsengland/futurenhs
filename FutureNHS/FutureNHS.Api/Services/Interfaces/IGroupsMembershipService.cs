using System.Security.Claims;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IGroupsMembershipService
    {
        Task<string> GetUserStatusForGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
    }
}
