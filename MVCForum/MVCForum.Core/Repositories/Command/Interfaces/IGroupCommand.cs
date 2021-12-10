namespace MvcForum.Core.Repositories.Command.Interfaces
{
    using MvcForum.Core.Models.Groups;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGroupCommand
    {
        Task<bool> UpdateAsync(GroupWriteViewModel model, string slug, CancellationToken cancellationToken = default(CancellationToken));
    }
}
