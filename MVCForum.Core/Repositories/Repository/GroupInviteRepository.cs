namespace MvcForum.Core.Repositories.Repository
{
    using Dapper;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class GroupInviteRepository : IGroupInviteRepository
    {

        private readonly IDbConnectionFactory _connectionFactory;

        public GroupInviteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {           
            const string query =
                @"
                    SELECT gi.Id,
                           gi.EmailAddress,
                           gi.GroupId,
                           gi.IsDeleted
                    FROM   GroupInvite gi
                    WHERE  GroupId = @groupId
                      AND  IsDeleted = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await  dbConnection.QueryAsync<GroupInviteViewModel>(commandDefinition);
            }
        }

        public async Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(MailAddress mailAddress, CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT gi.Id,
                           gi.EmailAddress,
                           gi.GroupId,
                           gi.IsDeleted
                    FROM   GroupInvite gi
                    WHERE  EmailAddress = @mailAddressValue
                      AND  IsDeleted = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new { mailAddressValue = mailAddress.Address }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.QueryAsync<GroupInviteViewModel>(commandDefinition);
            }
        }

        public async Task<GroupInviteViewModel> GetInviteForGroupAsync(Guid groupId, MailAddress mailAddress, CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT gi.Id,
                           gi.EmailAddress,
                           gi.GroupId,
                           gi.IsDeleted
                    FROM   GroupInvite gi
                    WHERE  EmailAddress = @mailAddressValue
	                  AND  GroupId = @groupId
                      AND  IsDeleted = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId, mailAddressValue = mailAddress.Address }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.QuerySingleOrDefaultAsync<GroupInviteViewModel>(commandDefinition);
            }
        }

        public async Task<bool> InviteExistsForGroupAsync(Guid groupId, MailAddress mailAddress, CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                    FROM   GroupInvite
                    WHERE  EmailAddress = @mailAddressValue
	                  AND  GroupId = @groupId;
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId, mailAddressValue = mailAddress.Address }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.ExecuteScalarAsync<bool>(commandDefinition);
            }
        }

        public async Task<bool> InviteExistsForMailAddressAsync(MailAddress mailAddress, CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                    FROM   GroupInvite
                    WHERE  EmailAddress = @mailAddressValue;
                ";

            var commandDefinition = new CommandDefinition(query, new { mailAddressValue = mailAddress.Address }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.ExecuteScalarAsync<bool>(commandDefinition);
            }
        }

        public async Task<bool> GroupMemberExistsAsync(Guid groupId, MailAddress mailAddress, CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                    FROM   MembershipUser
                    INNER JOIN GroupUser ON MembershipUser.Id = GroupUser.MembershipUser_Id
                    WHERE  MembershipUser.Email = @mailAddressValue 
	                  AND GroupUser.Group_Id = @groupId;
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId, mailAddressValue = mailAddress.Address }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.ExecuteScalarAsync<bool>(commandDefinition);
            }
        }

        public async Task<bool> MemberExistsAsync(MailAddress mailAddress, CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                    FROM   MembershipUser 
                    WHERE  MembershipUser.Email = @mailAddressValue;
                ";

            var commandDefinition = new CommandDefinition(query, new { mailAddressValue = mailAddress.Address }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.ExecuteScalarAsync<bool>(commandDefinition);
            }
        }

        public async Task<bool> IsMemberAdminAsync(string username, CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                        FROM   MembershipRole mr
                        JOIN MembershipUsersInRoles m
                            ON m.RoleIdentifier = id
						JOIN MembershipUser mu 
						    ON mu.Id = m.UserIdentifier
                        WHERE  mu.UserName = @username 
					    AND mr.RoleName = 'admin';                 
                ";

            var commandDefinition = new CommandDefinition(query, new { username }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.ExecuteScalarAsync<bool>(commandDefinition);
            }
        }
    }
}