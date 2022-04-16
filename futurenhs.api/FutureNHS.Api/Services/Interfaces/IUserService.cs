using System.Net.Mail;
using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid userId, uint offset, uint limit, string sort,
            CancellationToken cancellationToken);

        Task InviteMemberToGroupAndPlatformAsync(Guid userId, Guid? groupId, string email, CancellationToken cancellationToken);

        Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchUsers(Guid userId, string term, uint offset,
            uint limit, string sort, CancellationToken cancellationToken);
    }
}
