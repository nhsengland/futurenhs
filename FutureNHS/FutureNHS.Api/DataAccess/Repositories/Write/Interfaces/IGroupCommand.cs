using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;

public interface IGroupCommand
{
    Task UserJoinGroupAsync(GroupUserDto groupUser, CancellationToken cancellationToken = default);
    Task UserLeaveGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
    Task<Guid> GetGroupIdForSlugAsync(string slug, CancellationToken cancellationToken = default);
}