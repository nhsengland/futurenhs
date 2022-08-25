using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Identity.Request;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member;
using FutureNHS.Api.Models.Member.Request;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<MemberProfile> GetMemberAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken);
        Task<MemberDetails?> GetMemberByEmailAsync(string emailAddress, CancellationToken cancellationToken);
        Task<MemberInfoResponse> GetMemberInfoAsync(MemberIdentityRequest memberIdentityRequest, CancellationToken cancellationToken);
        Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid userId, uint offset, uint limit, string sort,
            CancellationToken cancellationToken);
        Task UpdateMemberAsync(Guid userId, Guid targetUserId, Stream requestBody, string? contentType, byte[] rowVersion, CancellationToken cancellationToken);
        Task<bool> IsMemberInvitedAsync(string emailAddress, CancellationToken cancellationToken);

        Task<Guid?> RegisterMemberAsync(MemberRegistrationRequest registrationRequest, CancellationToken cancellationToken);
    }
}
