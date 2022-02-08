using System.Data;
using Dapper;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.Write
{
    public class GroupCommand :IGroupCommand
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<GroupCommand> _logger;

        public GroupCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<GroupCommand> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

        }

        public async Task UserJoinGroupAsync(GroupUserDto groupUser,  CancellationToken cancellationToken = default)
        {
            const string query =
                 @" INSERT INTO  [dbo].[GroupUser]
                                 ([Id]
                                ,[Approved]
                                ,[Rejected]
                                ,[Locked]
                                ,[Banned]
                                ,[RequestToJoinDate]
                                ,[ApprovedToJoinDate]
                                ,[RequestToJoinReason]
                                ,[LockReason]
                                ,[BanReason]
                                ,[ApprovingMembershipUser_Id]
                                ,[MembershipRole_Id]
                                ,[MembershipUser_Id]
                                ,[Group_Id])
                    VALUES
                                (NEWID()
                                ,@Approved
                                ,@Rejected
                                ,@Locked
                                ,@Banned
                                ,@RequestToJoinDate
                                ,@ApprovedToJoinDate
                                ,@RequestToJoinReason
                                ,@LockReason
                                ,@BanReason
                                ,@ApprovingUser
                                ,@Role
                                ,@UserId
                                ,@GroupId)";

            var queryDefinition = new CommandDefinition(query, new
            {
                GroupId = groupUser.Group,
                UserId = groupUser.MembershipUser,
                Approved = groupUser.Approved,
                Rejected = groupUser.Rejected,
                Locked = groupUser.Locked,
                Banned = groupUser.Banned,
                RequestToJoinDate = groupUser.RequestToJoinDateUTC,
                ApprovedToJoinDate = groupUser.ApprovedDateUTC,
                RequestToJoinReason = groupUser.RequestToJoinReason,
                LockReason = groupUser.LockReason,
                BanReason = groupUser.BanReason,
                ApprovingUser = groupUser.ApprovingMembershipUser,
                Role = groupUser.MembershipRole

            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to join group was not added", queryDefinition);
                throw new DBConcurrencyException("Error: User request to join group was not added");
            }
        }

        public async Task UserLeaveGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == groupId) throw new ArgumentOutOfRangeException(nameof(groupId));

            const string query =
                 @" DELETE FROM     [dbo].[GroupUser]
                    WHERE           MembershipUser_Id = @UserId
                    AND             Group_Id = @GroupId";

            var queryDefinition = new CommandDefinition(query, new
            {
                GroupId = groupId,
                UserId = userId,


            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to leave group failed", queryDefinition);
                throw new DBConcurrencyException("Error: User request to leave group failed");
            }
        }

        public async Task<Guid> GetGroupIdForSlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            const string query =
                @$" SELECT
                                Id
          
                    FROM        [Group] 
                    WHERE       Slug = @Slug ";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var id = await dbConnection.QuerySingleAsync<Guid>(query, new
            {
                Slug = slug,
            });

            return id;
        }

    }
}
