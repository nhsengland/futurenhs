using System.Security.Claims;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IGroupMembershipService
    {
        Task UserJoinGroupAsync(Guid userId, string slug, CancellationToken cancellationToken);
        Task UserLeaveGroupAsync(Guid userId, string slug, CancellationToken cancellationToken);
    }
}
