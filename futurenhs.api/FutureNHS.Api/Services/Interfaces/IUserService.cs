using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid userId, uint offset, uint limit, string sort,
            CancellationToken cancellationToken);
    }
}
