using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IUserCommand
    {
        Task CreateInviteUserAsync(GroupInviteDto entityLike, CancellationToken cancellationToken);
        Task UpdateUserAsync(MemberDto userDto, byte[] rowVersion, CancellationToken cancellationToken);
        Task<MemberData?> GetMemberAsync(Guid id, CancellationToken CancellationToken);
    }
}
