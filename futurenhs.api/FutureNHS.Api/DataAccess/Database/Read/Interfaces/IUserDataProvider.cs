using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IUserDataProvider
    {
        Task<MemberDetails?> GetMemberAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
