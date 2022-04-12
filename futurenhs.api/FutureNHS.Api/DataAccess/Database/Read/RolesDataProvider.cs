using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Permissions;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class RolesDataProvider : IRolesDataProvider
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<RolesDataProvider> _logger;

        public RolesDataProvider(IAzureSqlDbConnectionFactory connectionFactory,ILogger<RolesDataProvider> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

        }

        public async Task<UserAndGroupRoles> GetUserAndGroupUserRolesAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == groupId) throw new ArgumentOutOfRangeException(nameof(groupId));

            const string query =
                @"SELECT mr.RoleName
                   FROM MembershipUser mu 
                   JOIN MembershipUsersInRoles mir on mir.UserIdentifier = mu.Id
                   JOIN MembershipRole mr on mr.Id = mir.RoleIdentifier
                   WHERE mu.Id = @UserId AND
                   mu.IsApproved = 1 AND
                   mu.IsBanned = 0 AND
                   mu.IsLockedOut = 0;

                  SELECT mr.RoleName, gu.Approved, gu.Rejected, gu.Locked, gu.Banned
                   FROM MembershipUser mu
                   JOIN GroupUser gu on gu.MembershipUser_Id = mu.Id
                   JOIN [Group] g on g.Id = gu.Group_Id
                   JOIN MembershipRole mr on mr.Id = gu.MembershipRole_Id
                   WHERE mu.Id = @UserId AND 
                   g.Id = @GroupId AND
                   mu.IsApproved = 1 AND
                   mu.IsBanned = 0 AND
                   mu.IsLockedOut = 0;
                ";

            var queryDefinition = new CommandDefinition(query, new
            {
                GroupId = groupId,
                UserId = userId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            using var result = await dbConnection.QueryMultipleAsync(queryDefinition);

            var userRoles = new UserAndGroupRoles(await result.ReadAsync<string>(), await result.ReadAsync<GroupUserRole>());
            
            return userRoles;
        }

        public async Task<UserAndGroupRoles> GetUserAndGroupUserRolesAsync(Guid userId, string slug, CancellationToken cancellationToken = default)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            const string query =
                @"SELECT mr.RoleName
                   FROM MembershipUser mu 
                   JOIN MembershipUsersInRoles mir on mir.UserIdentifier = mu.Id
                   JOIN MembershipRole mr on mr.Id = mir.RoleIdentifier
                   WHERE mu.Id = @UserId AND
                   mu.IsApproved = 1 AND
                   mu.IsBanned = 0 AND
                   mu.IsLockedOut = 0;

                  SELECT mr.RoleName, gu.Approved, gu.Rejected, gu.Locked, gu.Banned
                   FROM MembershipUser mu
                   JOIN GroupUser gu on gu.MembershipUser_Id = mu.Id
                   JOIN [Group] g on g.Id = gu.Group_Id
                   JOIN MembershipRole mr on mr.Id = gu.MembershipRole_Id
                   WHERE mu.Id = @UserId AND 
                   g.Slug = @Slug AND
                   mu.IsApproved = 1 AND
                   mu.IsBanned = 0 AND
                   mu.IsLockedOut = 0;
                ";

            var queryDefinition = new CommandDefinition(query, new
            {
                Slug = slug,
                UserId = userId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            using var result = await dbConnection.QueryMultipleAsync(queryDefinition);

            var userRoles = new UserAndGroupRoles(await result.ReadAsync<string>(), await result.ReadAsync<GroupUserRole>());

            return userRoles;
        }

        public async Task<IEnumerable<string>?> GetUserRolesAsync(Guid userId,CancellationToken cancellationToken = default)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            const string query =
                @"SELECT mr.RoleName 
                   FROM MembershipUser mu 
                   JOIN MembershipUsersInRoles mir on mir.UserIdentifier = mu.Id
                   JOIN MembershipRole mr on mr.Id = mir.RoleIdentifier
                   WHERE mu.Id = @UserId AND
                   mu.IsApproved = 1 AND
                   mu.IsBanned = 0 AND
                   mu.IsLockedOut = 0;
                ";

            var queryDefinition = new CommandDefinition(query, new
            {
                UserId = userId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var result = await dbConnection.QueryAsync<string>(queryDefinition);

            return result;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(RoleDto.Id)}]     = Id,
                                [{nameof(RoleDto.Name)}]   = RoleName 
          
                    FROM        [MembershipRole];";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, cancellationToken: cancellationToken);

            return await dbConnection.QueryAsync<RoleDto>(commandDefinition);
        }
    }
}
