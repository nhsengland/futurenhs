namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGroupInviteRepository
    {
        Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(Guid groupId, CancellationToken cancellationToken);

        Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(string emailAddress, CancellationToken cancellationToken);

        Task<GroupInviteViewModel> GetInviteForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken);

        Task<bool> InviteExistsForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken);

        Task<bool> GroupMemberExistsAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken);

        Task<bool> MemberExistsAsync(string emailAddress, CancellationToken cancellationToken);

        Task<bool> IsMemberAdminAsync(string username, CancellationToken cancellationToken);

    }
}
