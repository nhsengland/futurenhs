using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IUserDataProvider
    {
        Task<MemberDetails?> GetMemberAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<MemberDetails?> GetMemberByEmailAsync(string emailAddress, CancellationToken cancellationToken = default);
        Task<MemberInfoResponse> GetMemberInfoAsync(string subjectId, CancellationToken cancellationToken = default);
        Task<bool> IsMemberInvitedAsync(string emailAddress, CancellationToken cancellationToken = default);
        Task<MemberProfile> GetMemberProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
