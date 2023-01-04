using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Models.Member;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.DataAccess.Models.Identity;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IUserCommand
    {
        Task<Identity> GetMemberIdentityAsync(string subjectId, CancellationToken cancellationToken);
        Task<UserDto> GetUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<Guid> CreateInviteUserAsync(PlatformInviteDto entityLike, CancellationToken cancellationToken);
        Task UpdateUserAsync(MemberDto userDto, byte[] rowVersion, CancellationToken cancellationToken);
        Task<MemberProfile> GetMemberAsync(Guid id, CancellationToken CancellationToken);
        Task<MemberRole> GetMembershipUsersInRoleAsync(Guid userId, CancellationToken cancellationToken);
        Task UpdateUserRoleAsync(MemberRoleUpdate memberRoleUpdate, byte[] rowVersion, CancellationToken cancellationToken);
        Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchUsers(string term, uint offset, uint limit,
            string sort, CancellationToken cancellationToken);
        Task<MemberDetails?> GetMemberByEmailAsync(string emailAddress, CancellationToken cancellationToken = default);
        Task<MemberInfoResponse> GetMemberInfoAsync(string subjectId, CancellationToken cancellationToken = default);
        Task<Guid> RegisterUserAsync(MemberDto user, string subjectId, string issuer, string defaultRole, CancellationToken cancellationToken);
        Task MapIdentityToExistingUserAsync(Guid membershipUserId, string subjectId, string issuer, CancellationToken cancellationToken);
        Task RecordUserActivityAsync(Guid userId, DateTime activityDate, CancellationToken cancellationToken);
        Task<Guid> CreateInviteGroupUserAsync(GroupInviteDto userInvite, CancellationToken cancellationToken);
        Task RedeemPlatformInviteAsync(Guid userId, string email, CancellationToken cancellationToken);
    }
}
