﻿namespace MvcForum.Core.Repositories.Repository
{
    using Dapper;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class GroupInviteRepository : IGroupInviteRepository
    {

        private readonly IDbConnectionFactory _connectionFactory;

        public GroupInviteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT gi.Id,
                           gi.EmailAddress,
                           gi.GroupId,
                           gi.IsDeleted
                    FROM   GroupInvite gi
                    WHERE  GroupId = @groupId
                      AND  IsDeleted = 0
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId }, cancellationToken: cancellationToken);

            using (var multipleResults = await dbConnection.QueryMultipleAsync(commandDefinition))
            {
                var results = multipleResults.Read<GroupInviteViewModel>();
                return results;
            }
        }

        public async Task<IEnumerable<GroupInviteViewModel>> GetInvitesForGroupAsync(string emailAddress, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT gi.Id,
                           gi.EmailAddress,
                           gi.GroupId,
                           gi.IsDeleted
                    FROM   GroupInvite gi
                    WHERE  EmailAddress = @emailAddress
                      AND  IsDeleted = 0
                ";

            var commandDefinition = new CommandDefinition(query, new { emailAddress }, cancellationToken: cancellationToken);


            using (var multipleResults = await dbConnection.QueryMultipleAsync(commandDefinition))
            {
                var results = multipleResults.Read<GroupInviteViewModel>();
                return results;
            }
        }

        public async Task<GroupInviteViewModel> GetInviteForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT gi.Id,
                           gi.EmailAddress,
                           gi.GroupId,
                           gi.IsDeleted
                    FROM   GroupInvite gi
                    WHERE  EmailAddress = @emailAddress
	                  AND  GroupId = @groupId
                      AND  IsDeleted = 0
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId, emailAddress }, cancellationToken: cancellationToken);

            return (await dbConnection.QueryAsync<GroupInviteViewModel>(commandDefinition)).SingleOrDefault();
        }

        public async Task<bool> InviteExistsForGroupAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                    FROM   GroupInvite
                    WHERE  EmailAddress = @emailAddress
	                  AND  GroupId = @groupId
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId, emailAddress }, cancellationToken: cancellationToken);

            return (await dbConnection.QueryAsync<bool>(commandDefinition)).SingleOrDefault();
        }

        public async Task<bool> GroupMemberExistsAsync(Guid groupId, string emailAddress, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                    FROM   MembershipUser
                    INNER JOIN GroupUser ON MembershipUser.Id = GroupUser.MembershipUser_Id
                    WHERE  MembershipUser.Email = @emailAddress 
	                  AND GroupUser.Group_Id = @groupId
                ";

            var commandDefinition = new CommandDefinition(query, new { groupId, emailAddress }, cancellationToken: cancellationToken);
            
            return (await dbConnection.QueryAsync<bool>(commandDefinition)).SingleOrDefault();
        }

        public async Task<bool> MemberExistsAsync(string emailAddress, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                    FROM   MembershipUser 
                    WHERE  MembershipUser.Email = @emailAddress 
                ";

            var commandDefinition = new CommandDefinition(query, new { emailAddress }, cancellationToken: cancellationToken);

            return (await dbConnection.QueryAsync<bool>(commandDefinition)).SingleOrDefault();
        }

        public async Task<bool> IsMemberAdminAsync(string username, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT CAST(COUNT(1) AS BIT)
                        FROM   MembershipRole mr
                        JOIN MembershipUsersInRoles m
                            ON m.RoleIdentifier = id
						JOIN MembershipUser mu 
						    ON mu.Id = m.UserIdentifier
                        WHERE  mu.UserName = @username 
					    AND mr.RoleName = 'admin'                   
                ";

            var commandDefinition = new CommandDefinition(query, new { username }, cancellationToken: cancellationToken);

            return (await dbConnection.QueryAsync<bool>(commandDefinition)).SingleOrDefault();
        }
    }
}