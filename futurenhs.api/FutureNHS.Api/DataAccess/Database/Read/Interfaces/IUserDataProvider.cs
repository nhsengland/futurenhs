using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Member;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IUserDataProvider
    {
        Task<MemberDetails?> GetMemberAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<MemberProfile> GetMemberProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
