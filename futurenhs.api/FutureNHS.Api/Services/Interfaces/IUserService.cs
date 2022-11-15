using FutureNHS.Api.DataAccess.Models.Identity;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<MemberProfile> GetMemberAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken);
        Task<MemberProfile> GetMemberProfileAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken);
        Task<MemberDetails?> GetMemberByEmailAsync(string emailAddress, CancellationToken cancellationToken);
        Task<MemberInfoResponse> GetMemberInfoAsync(string subjectId, string? emailAddress, CancellationToken cancellationToken);
        Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid userId, uint offset, uint limit, string sort,
            CancellationToken cancellationToken);
        Task UpdateMemberAsync(Guid userId, Guid targetUserId, Stream requestBody, string? contentType, byte[] rowVersion, CancellationToken cancellationToken);
        Task<bool> CheckMemberCanRegisterAsync(string emailAddress, CancellationToken cancellationToken);
        Task<Guid?> GetInviteIdForEmailAsync(string emailAddress, CancellationToken cancellationToken, Guid? groupId = null);
        Task<Guid?> GetGroupInviteIdForMemberAsync(Guid targetUserId, Guid groupId, CancellationToken cancellationToken);
        Task<Identity> GetMemberIdentityAsync(string subjectId, CancellationToken cancellationToken);

    }
}
