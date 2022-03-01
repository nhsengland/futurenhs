using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write.Interfaces
{
    public interface IRolesCommand
    {
        Task<RoleDto> GetRoleAsync(string name, CancellationToken cancellationToken = default);
    }
}
