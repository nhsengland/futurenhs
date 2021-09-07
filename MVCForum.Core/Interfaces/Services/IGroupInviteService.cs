namespace MvcForum.Core.Interfaces.Services
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public  interface IGroupInviteService
    {
        Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(Guid groupId, CancellationToken cancellationToken);
        Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(string emailAddress, CancellationToken cancellationToken);
        Task<GroupInviteViewModel> GetInviteForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken);
        Task<bool> InviteExistsForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken);
        Task<bool> MemberExistsInGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken);
        Task<bool> MemberExistsInSystemAsync(string emailAddress, CancellationToken cancellationToken);
        Task<bool> IsMemberAdminAsync(string username, CancellationToken cancellationToken);
        Task<Guid> CreateInviteAsync(GroupInviteViewModel invite, CancellationToken cancellationToken);
        Task<bool> DeleteInviteAsync(Guid inviteId, CancellationToken cancellationToken);
    }
}
