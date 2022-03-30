using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IUserCommand
    {
        Task CreateInviteUserAsync(GroupInviteDto entityLike, CancellationToken cancellationToken);
    }
}
