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

        public async Task<RoleDto> GetRoleAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentOutOfRangeException(nameof(name));

            const string query =
                @$" SELECT
                                [{nameof(RoleDto.Id)}]                   = Id,
                                [{nameof(RoleDto.Name)}]                 = RoleName 
          
                    FROM        MembershipRole   
                    WHERE       RoleName = @RoleName ";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var role = await dbConnection.QuerySingleAsync<RoleDto>(query, new
            {
                RoleName = name,
            });

            return role;
        }
    }
}
