using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class RolesCommand : IRolesCommand
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<RolesCommand> _logger;

        public RolesCommand(IAzureSqlDbConnectionFactory connectionFactory,ILogger<RolesCommand> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

        }

        public async Task<RoleDto> GetRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

            const string query =
                @$" SELECT
                                [{nameof(RoleDto.Id)}]     = Id,
                                [{nameof(RoleDto.Name)}]   = RoleName 
          
                    FROM        MembershipRole   
                    WHERE       RoleName = @RoleName;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var role = await dbConnection.QuerySingleAsync<RoleDto>(query, new
            {
                RoleName = roleName,
            });

            return role;
        }

        public async Task<RoleDto> GetRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            if (Guid.Empty == roleId) throw new ArgumentOutOfRangeException(nameof(roleId));

            const string query =
                @$" SELECT
                                [{nameof(RoleDto.Id)}]     = Id,
                                [{nameof(RoleDto.Name)}]   = RoleName 
          
                    FROM        MembershipRole   
                    WHERE       Id = @RoleId;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var role = await dbConnection.QuerySingleAsync<RoleDto>(query, new
            {
                RoleId = roleId,
            });

            return role;
        }
    }
}
