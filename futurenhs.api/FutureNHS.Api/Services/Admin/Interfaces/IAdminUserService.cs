using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Member;

namespace FutureNHS.Api.Services.Admin.Interfaces
{
    public interface IAdminUserService
    {
        Task<MemberProfile> GetMemberAsync(Guid adminUserId, Guid targetUserId, CancellationToken cancellationToken);
        Task<(uint, IEnumerable<DataAccess.Models.User.Member>)> GetMembersAsync(Guid adminUserId, uint offset, uint limit, string sort,
            CancellationToken cancellationToken);
        Task<IEnumerable<RoleDto>> GetMemberRolesAsync(Guid adminUserId, CancellationToken cancellationToken);
        Task<MemberRole> GetMemberRoleAsync(Guid adminUserId, Guid targetUserId, CancellationToken cancellationToken);
        Task UpdateMemberRoleAsync(Guid adminUserId, MemberRoleUpdate memberRoleUpdate, byte[] rowVersion, CancellationToken cancellationToken);
        Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchMembersAsync(Guid adminUserId, string term, uint offset,
            uint limit, string sort, CancellationToken cancellationToken);
    }
}
