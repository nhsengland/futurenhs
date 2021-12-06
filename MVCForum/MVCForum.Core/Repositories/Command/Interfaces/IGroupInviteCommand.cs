namespace MvcForum.Core.Repositories.Command.Interfaces
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGroupInviteCommand
    {
        Task<Guid> CreateInviteAsync(GroupInviteViewModel invite, CancellationToken cancellationToken);
        Task<bool> DeleteInviteAsync(Guid inviteId, CancellationToken cancellationToken);
    }
}
