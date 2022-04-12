using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Member;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<MemberProfile> GetMemberAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken);
        Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid userId, uint offset, uint limit, string sort,
            CancellationToken cancellationToken);
        Task UpdateMemberAsync(Guid userId, Guid targetUserId, Stream requestBody, string? contentType, byte[] rowVersion, CancellationToken cancellationToken);
    }
}
