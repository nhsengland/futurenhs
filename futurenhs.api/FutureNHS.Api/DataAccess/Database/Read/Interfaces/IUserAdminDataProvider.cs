using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Database.Read.Interfaces
{
    public interface IUserAdminDataProvider
    {
        Task<(uint, IEnumerable<Member>)> GetMembersAsync(uint offset, uint limit, string sort,
            CancellationToken cancellationToken = default);

    }
}
