using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IUserCommand
    {
        Task<UserDto> GetUserAsync(Guid userId, CancellationToken cancellationToken);
        Task CreateInviteUserAsync(GroupInviteDto entityLike, CancellationToken cancellationToken);

        Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchUsers(string term, uint offset, uint limit,
            string sort, CancellationToken cancellationToken);
    }
}
