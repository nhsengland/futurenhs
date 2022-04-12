using System.Data;
using Dapper;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class GroupCommand : IGroupCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<GroupCommand> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public GroupCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<GroupCommand> logger, IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task UserJoinGroupAsync(GroupUserDto groupUser, CancellationToken cancellationToken = default)
        {
            const string query =
                 @" INSERT INTO  [dbo].[GroupUser]
                                 ([Id]
                                ,[Approved]
                                ,[Rejected]
                                ,[Locked]
                                ,[Banned]
                                ,[RequestToJoinDateUTC]
                                ,[ApprovedToJoinDateUTC]
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
                _logger.LogError($"Error: User request to join group was not added User:{0}, Group:{1} ", groupUser.MembershipUser, groupUser.Group);
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

        public async Task<GroupData> GetGroupAsync(string slug, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT 
                                [{nameof(GroupData.Id)}]                = g.Id, 
                                [{nameof(GroupData.Name)}]              = g.Name,
                                [{nameof(GroupData.Strapline)}]         = g.Subtitle,
                                [{nameof(GroupData.ThemeId)}]           = g.ThemeId, 
                                [{nameof(GroupData.ImageId)}]           = g.ImageId, 
                                [{nameof(GroupData.IsPublic)}]          = g.PublicGroup, 
                                [{nameof(GroupData.Slug)}]              = g.Slug, 
                                [{nameof(GroupData.RowVersion)}]        = g.RowVersion, 
                                [{nameof(GroupData.Image.Id)}]          = image.Id,  
                                [{nameof(GroupData.Image.Height)}]      = image.Height, 
                                [{nameof(GroupData.Image.Width)}]       = image.Width,
                                [{nameof(GroupData.Image.FileName)}]    = image.FileName,
                                [{nameof(GroupData.Image.MediaType)}]   = image.MediaType
				    
                    FROM        [Group] g
                    LEFT JOIN   Image image 
                    ON          image.Id = g.ImageId  
                    WHERE       g.Slug = @Slug 
                    AND         g.IsDeleted = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryAsync<GroupData, Image, GroupData>(query,
                (group, image) =>
                {
                    if (image is not null)
                    {
                        group = @group with { Image = new ImageData(image, _options) };
                    }

                    return group;
                }, new
                {
                    Slug = slug
                }, splitOn: "id");

            var group = reader.FirstOrDefault() ?? throw new NotFoundException("Group not found.");

            return group;
        }

        public async Task UpdateGroupAsync(GroupDto groupDto, CancellationToken cancellationToken = default)
        {
            const string query =
                 @" UPDATE       [dbo].[Group]
                    SET
                                 [Name]         = @Name,
                                 [Subtitle]     = @Subtitle,
                                 [ThemeId]      = @Theme,
                                 [ImageId]      = @Image
                    WHERE 
                                 [Slug]         = @Slug
                    AND          [RowVersion]   = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                Slug = groupDto.Slug,
                Name = groupDto.Name,
                Subtitle = groupDto.Strapline,
                Theme = groupDto.ThemeId,
                Image = groupDto.ImageId,
                IsPublic = groupDto.IsPublic,
                RowVersion = groupDto.RowVersion,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError($"Error: Unable to update Group:{1} ", groupDto.Id);
                throw new DBConcurrencyException("Error: Unable to update group");
            }
        }

        public async Task<Guid?> GetGroupIdForSlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            const string query =
                @$" SELECT
                                Id
          
                    FROM        [Group] 
                    WHERE       Slug = @Slug ";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var id = await dbConnection.QueryFirstOrDefaultAsync<Guid?>(query, new
            {
                Slug = slug,
            });

            return id;
        }

        public async Task CreateGroupSiteAsync(GroupSiteDto groupSite, CancellationToken cancellationToken)
        {
            const string query =
                 @" INSERT INTO  [dbo].[GroupSite]
                                 ([Id]
                                 ,[GroupId]
                                 ,[ContentRootId]
                                 ,[CreatedBy]
                                 ,[CreatedAtUTC]
                                 ,[ModifiedBy])
                    VALUES
                                 (NEWID()
                                 ,@GroupId
                                 ,@ContentRootId
                                 ,@CreatedBy
                                 ,@CreatedAtUTC
                                 ,@ModifiedBy)";

            var queryDefinition = new CommandDefinition(query, new
            {
                GroupId = groupSite.GroupId,
                ContentRootId = groupSite.ContentRootId,
                CreatedBy = groupSite.CreatedBy,
                CreatedAtUTC = groupSite.CreatedAtUTC,
                ModifiedBy = groupSite.ModifiedBy,
                ModifiedAtUTC = groupSite.ModifiedAtUTC,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to create a group site was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to create a group site was not successful");
            }
        }

        public async Task DeleteGroupSiteAsync(Guid contentId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == contentId) throw new ArgumentOutOfRangeException(nameof(contentId));

            const string query =
                 @" DELETE FROM     [dbo].[GroupSite]
                    WHERE           ContentRootId = @ContentId;";

            var queryDefinition = new CommandDefinition(query, new
            {
                ContentRootId = contentId,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to delete group site failed", queryDefinition);
                throw new DBConcurrencyException("Error: User request to delete group site failed");
            }
        }

        public async Task<GroupUserDto> GetGroupUserAsync(Guid groupUserId, Guid groupId, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(GroupUserDto.Id)}]                            = groupUser.Id,
                                [{nameof(GroupUserDto.Approved)}]                      = groupUser.Approved, 
                                [{nameof(GroupUserDto.Rejected)}]                      = groupUser.Rejected,
                                [{nameof(GroupUserDto.Locked)}]                        = groupUser.Locked,
                                [{nameof(GroupUserDto.Banned)}]                        = groupUser.Banned, 
                                [{nameof(GroupUserDto.RequestToJoinDateUTCAsString)}]  = FORMAT(groupUser.RequestToJoinDateUTC,'yyyy-MM-ddTHH:mm:ssZ'), 
                                [{nameof(GroupUserDto.ApprovedDateUTCAsString)}]       = FORMAT(groupUser.ApprovedToJoinDateUTC, 'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(GroupUserDto.RequestToJoinReason)}]           = groupUser.RequestToJoinReason,
                                [{nameof(GroupUserDto.LockReason)}]                    = groupUser.LockReason,
                                [{nameof(GroupUserDto.ApprovingMembershipUser)}]       = groupUser.ApprovingMembershipUser_Id,
                                [{nameof(GroupUserDto.MembershipRole)}]                = groupUser.MembershipRole_Id,
                                [{nameof(GroupUserDto.MembershipUser)}]                = groupUser.MembershipUser_Id,
                                [{nameof(GroupUserDto.Group)}]                         = groupUser.Group_Id,
                                [{nameof(GroupUserDto.RowVersion)}]                    = groupUser.RowVersion
                                

                    FROM        [GroupUser] groupUser
                    WHERE       groupUser.Id = @GroupUserId
                    AND         groupUser.Group_Id = @GroupId;";

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                GroupUserId = groupUserId,
                GroupId = groupId
            }, cancellationToken: cancellationToken);

            return await dbConnection.QuerySingleOrDefaultAsync<GroupUserDto>(commandDefinition);
        }

        public async Task<IEnumerable<GroupRoleDto>> GetAllGroupRolesAsync(CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(GroupRoleDto.Id)}]     = Id,
                                [{nameof(GroupRoleDto.Name)}]   = RoleName 
          
                    FROM        [MembershipRole];";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var queryDefinition = new CommandDefinition(query, cancellationToken: cancellationToken);

            return await dbConnection.QueryAsync<GroupRoleDto>(queryDefinition);
        }

        public async Task UpdateUserGroupRolesAsync(Guid groupUserId, Guid groupRoleId, byte[] rowVersion, CancellationToken cancellationToken = default)
        {
            const string query =
                @" UPDATE       [dbo].[GroupUser]
                   SET
                                [MembershipRole_Id]         = @groupRoleId
                   WHERE 
                                [Id]                        = @groupUserId
                   AND          [RowVersion]                = @RowVersion";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                GroupUserId = groupUserId,
                GroupRoleId = groupRoleId,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            await dbConnection.ExecuteAsync(commandDefinition);
        }

        public async Task DeleteUserFromGroupAsync(Guid groupUserId, byte[] rowVersion, CancellationToken cancellationToken = default)
        {
            const string query =
                @" DELETE       
                   FROM         [dbo].[GroupUser]
                        
                   WHERE        [Id]                  = @groupUserId
                   AND          [RowVersion]          = @RowVersion";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                GroupUserId = groupUserId,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            await dbConnection.ExecuteAsync(commandDefinition);
        }
    }
}
