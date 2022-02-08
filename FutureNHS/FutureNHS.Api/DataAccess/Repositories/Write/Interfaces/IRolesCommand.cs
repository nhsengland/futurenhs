using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Repositories.Write.Interfaces
{
    public interface IRolesCommand
    {
        Task<RoleDto> GetRoleAsync(string name, CancellationToken cancellationToken = default);
    }
}
