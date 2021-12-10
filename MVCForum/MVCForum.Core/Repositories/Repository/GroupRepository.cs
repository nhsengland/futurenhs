namespace MvcForum.Core.Repositories.Repository
{
    using MvcForum.Core.Models.Groups;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using Dapper;
    using System.Data.SqlClient;

    public sealed class GroupRepository : IGroupRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GroupRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<GroupViewModel> GetGroupAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id));

            var sql = @"SELECT  [Id],
                                [Name],
                                [Description],
                                [Introduction],
                                [Slug],
                                [Image],
                                [PublicGroup],
                                [IsDeleted]
                        FROM [dbo].[Group] 
                        WHERE [Id] = @id
                        AND [IsDeleted] = 0";

            using (var conn = this._connectionFactory.CreateReadOnlyConnection())
            {
                return await conn.QuerySingleAsync<GroupViewModel>(sql, new { id = id });
            }
        }

        public async Task<GroupViewModel> GetGroupAsync(string slug, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            var sql = @"SELECT  [Id],
                                [Name],
                                [Description],
                                [Introduction],
                                [Slug],
                                [Image],
                                [PublicGroup],
                                [IsDeleted]
                        FROM [dbo].[Group] 
                        WHERE [slug] = @slug
                        AND [IsDeleted] = 0";

            using (var conn = this._connectionFactory.CreateReadOnlyConnection())
            {
                return await conn.QuerySingleAsync<GroupViewModel>(sql, new { slug = slug });
            }
        }

        public bool UserIsAdmin(string groupSlug, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) throw new ArgumentNullException(nameof(groupSlug));
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(groupSlug));

            var (MembershipRole, GroupRole) = GetUserRoles(groupSlug, userId);

            return MembershipRole?.ToLower() == "admin" || GroupRole?.ToLower() == "admin";
        }

        public bool UserHasGroupAccess(string groupSlug, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) throw new ArgumentNullException(nameof(groupSlug));
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(groupSlug));

            var (MembershipRole, GroupRole) = GetUserRoles(groupSlug, userId);

            return MembershipRole?.ToLower() == "admin" || GroupRole?.ToLower() == "admin" || GroupRole?.ToLower() == "standard members";
        }

        private (string MembershipRole, string GroupRole) GetUserRoles(string groupSlug, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) throw new ArgumentNullException(nameof(groupSlug));
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(groupSlug));

            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT rolename AS MemberRole
                    FROM   membershiprole
                           JOIN membershipusersinroles m
                             ON m.roleidentifier = id
                    WHERE  m.useridentifier = @userId

                    SELECT mr.rolename AS GroupRole
                    FROM   groupuser gu
                           JOIN membershiprole mr
                             ON gu.membershiprole_id = mr.id
                           JOIN membershipusersinroles mur
                             ON mur.useridentifier = gu.membershipuser_id
                           JOIN [group] g
                             ON gu.group_id = g.id
                    WHERE  g.slug = @groupSlug
                           AND gu.membershipuser_id = @userId
                           AND gu.approved = 1
                           AND gu.banned = 0
                           AND gu.locked = 0
                ";

            using (var result = dbConnection.QueryMultiple(query, new { groupSlug, userId }))
            {
                var membershipRole = result.Read<string>().FirstOrDefault();
                var groupRole = result.Read<string>().FirstOrDefault();

                return (membershipRole, groupRole);
            }
        }
    }
}
