using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces;

public interface IGroupCommand
{
    Task UserJoinGroupAsync(GroupUserDto groupUser, CancellationToken cancellationToken = default);
    Task UserLeaveGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
    Task<Guid?> GetGroupIdForSlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<GroupData> GetGroupAsync(string slug, CancellationToken cancellationToken = default);
    Task EditGroupAsync(GroupDto groupDto, CancellationToken cancellationToken = default);
}