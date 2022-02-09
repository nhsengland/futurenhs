namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using MvcForum.Core.Models.Groups;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGroupRepository
    {
        Task<GroupViewModel> GetGroupAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        Task<GroupViewModel> GetGroupAsync(string slug, CancellationToken cancellationToken = default(CancellationToken));

        GroupViewModel GetGroup(string slug);

        bool UserIsAdmin(string groupSlug, Guid userId);

        bool UserHasGroupAccess(string groupSlug, Guid userId);
    }
}
