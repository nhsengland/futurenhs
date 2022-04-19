using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class GroupDataProvider : IGroupDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<GroupDataProvider> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public GroupDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<GroupDataProvider> logger,
            IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid id, uint offset, uint limit, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            uint totalCount;

            IEnumerable<GroupSummary> groups;

            const string query =
                @"SELECT g.Id AS Id, g.ThemeId AS ThemeId, g.Slug AS Slug, g.Name AS NameText, g.Subtitle AS StraplineText, 
				(SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = g.Id AND groupUser.Approved = 1 ) AS MemberCount, 
				(SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = g.Id) AS DiscussionCount,
                image.Id, image.Height AS Height, image.Width AS Width, image.FileName AS FileName, image.MediaType AS MediaType
				FROM [Group] g
                JOIN GroupUser groupUser ON groupUser.Group_Id = g.Id
                LEFT JOIN Image image ON image.Id = g.ImageId 
                WHERE g.IsDeleted = 0 AND groupUser.MembershipUser_Id = @UserId AND groupUser.Approved = 1
                ORDER BY g.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) FROM GroupUser groupUser
                JOIN [Group] g ON g.Id = groupUser.Group_Id
                WHERE g.IsDeleted = 0 AND groupUser.MembershipUser_Id = @UserId AND groupUser.Approved = 1";
            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                using var reader = await dbConnection.QueryMultipleAsync(query, new
                {
                    Offset = Convert.ToInt32(offset),
                    Limit = Convert.ToInt32(limit),
                    UserId = id
                });
                groups = reader.Read<GroupSummary, ImageData, GroupSummary>(
                    (group, image) =>
                    {
                        if (image is not null)
                        {
                            var groupWithImage = group with { Image = new ImageData(image, _options) };

                            return groupWithImage;
                        }

                        return group;

                    }, splitOn: "id");

                totalCount = await reader.ReadFirstAsync<uint>();
            }

            return (totalCount, groups);
        }

        public async Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> DiscoverGroupsForUserAsync(Guid id, uint offset, uint limit, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            uint totalCount;

            IEnumerable<GroupSummary> groups;

            const string query =
                @"SELECT g.Id AS Id, g.ThemeId AS ThemeId, g.Slug AS Slug, g.Name AS NameText, g.Subtitle AS StraplineText, 
				(SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = g.Id AND groupUser.Approved = 1 ) AS MemberCount, 
				(SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = g.Id) AS DiscussionCount,
                image.Id, image.Height AS Height, image.Width AS Width, image.FileName AS FileName, image.MediaType AS MediaType
				FROM [Group] g    
                LEFT JOIN Image image ON image.Id = g.ImageId  
                WHERE g.IsDeleted = 0
				AND NOT EXISTS (select gu.Group_Id from GroupUser gu where  gu.MembershipUser_Id = @UserId AND gu.Group_Id = g.Id AND gu.Approved = 1)
                ORDER BY g.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) FROM [Group] g
                WHERE g.IsDeleted = 0
				AND NOT EXISTS (select gu.Group_Id from GroupUser gu where  gu.MembershipUser_Id = @UserId AND gu.Group_Id = g.Id AND gu.Approved = 1)";

            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                using var reader = await dbConnection.QueryMultipleAsync(query, new
                {
                    Offset = Convert.ToInt32(offset),
                    Limit = Convert.ToInt32(limit),
                    UserId = id
                });
                groups = reader.Read<GroupSummary, ImageData, GroupSummary>(
                    (group, image) =>
                    {
                        if (image is not null)
                        {
                            var groupWithImage = group with { Image = new ImageData(image, _options) };

                            return groupWithImage;
                        }

                        return group;

                    }, splitOn: "id");

                totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());
            }

            return (totalCount, groups);
        }

        public async Task<Group?> GetGroupAsync(string slug, Guid userId, CancellationToken cancellationToken = default)
        {
            const string query =
                @"SELECT g.Id AS Id, g.ThemeId AS ThemeId, g.Slug AS Slug, g.Name AS Name, g.Subtitle AS Strapline, g.PublicGroup AS IsPublic,( SELECT      CASE 
                                                                                    WHEN        groupUser.MembershipUser_Id = @UserId
                                                                                    AND         groupUser.Approved = 1
                                                                                    AND         groupUser.Rejected = 0
                                                                                    AND         groupUser.Locked = 0
                                                                                    AND         groupUser.Banned = 0
                                                                                    THEN        CAST(1 as bit) 
                                                                                    ELSE        CAST(0 as bit) 
                                                                                    END
                                                                                  ) AS IsMember,		
                image.Id, image.Height AS Height, image.Width AS Width, image.FileName AS FileName,  image.MediaType AS MediaType
				FROM [Group] g
                LEFT JOIN Image image ON image.Id = g.ImageId  
                LEFT JOIN GroupUser groupUser ON GroupUser.Group_Id = g.Id  
                WHERE g.Slug = @Slug AND g.IsDeleted = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryAsync<Group, Image, Group>(query,
                (group, image) =>
                {
                    if (image is not null)
                    {
                        var groupWithImage = @group with { Image = new ImageData(image, _options) };

                        return groupWithImage;
                    }

                    return @group;

                }, new
                {
                    Slug = slug,
                    UserId = userId
                }, splitOn: "id");

            var group = reader.FirstOrDefault() ?? throw new NotFoundException("Group not found.");

            return group;
        }

        public async Task<(uint, IEnumerable<GroupMember>)> GetGroupMembersAsync(string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @$" SELECT
                                [{nameof(GroupMember.Id)}]                   = member.Id,
                                [{nameof(GroupMember.Slug)}]                 = member.Slug, 
                                [{nameof(GroupMember.Name)}]                 = member.FirstName + ' ' +  member.Surname, 
                                [{nameof(GroupMember.DateJoinedUtc)}]        = FORMAT(groupUser.ApprovedToJoinDateUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(GroupMember.LastLoginUtc)}]         = FORMAT(member.LastLoginDateUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(GroupMember.Role)}]                 = memberRoles.RoleName

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    JOIN        MembershipUser member 
                    ON          member.Id = groupUser.MembershipUser_Id
                    JOIN        MembershipRole memberRoles 
                    ON          memberRoles.Id = groupUser.MembershipRole_Id 
                    WHERE       groups.Slug = @Slug
                    AND         groupUser.Approved = 1
                    ORDER BY    RoleName asc, Name asc

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    WHERE       groups.Slug = @Slug
                    AND         groupUser.Approved = 1;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = slug
            });

            var members = await reader.ReadAsync<GroupMember>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, members);
        }

        public async Task<(uint, IEnumerable<PendingGroupMember>)> GetPendingGroupMembersAsync(string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @$" SELECT
                                [{nameof(PendingGroupMember.Id)}]                   = member.Id,
                                [{nameof(PendingGroupMember.Slug)}]                 = member.Slug, 
                                [{nameof(PendingGroupMember.Name)}]                 = member.FirstName + ' ' +  member.Surname, 
                                [{nameof(PendingGroupMember.ApplicationDateUtc)}]   = FORMAT(groupUser.RequestToJoinDateUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(PendingGroupMember.LastLoginUtc)}]         = FORMAT(member.LastLoginDateUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(PendingGroupMember.Email)}]                = member.Email

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    JOIN        MembershipUser member 
                    ON          member.Id = groupUser.MembershipUser_Id      
                    WHERE       groups.Slug = @Slug
                    AND         groupUser.Approved = 0
                    AND         groupUser.Rejected = 0
                    AND         groupUser.Locked = 0
                    AND         groupUser.Banned = 0
                    ORDER BY    groupUser.RequestToJoinDateUTC desc

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    WHERE       groups.Slug = @Slug
                    AND         groupUser.Approved = 0
                    AND         groupUser.Rejected = 0
                    AND         groupUser.Locked = 0
                    AND         groupUser.Banned = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = slug
            });

            var members = await reader.ReadAsync<PendingGroupMember>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, members);
        }

        public async Task<GroupMemberDetails?> GetGroupMemberAsync(string slug, Guid userId, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(GroupMemberDetails.Id)}]                   = member.Id,
                                [{nameof(GroupMemberDetails.Slug)}]                 = member.Slug, 
                                [{nameof(GroupMemberDetails.FirstName)}]            = member.FirstName,
                                [{nameof(GroupMemberDetails.LastName)}]             = member.Surname,
                                [{nameof(GroupMemberDetails.Initials)}]             = member.Initials, 
                                [{nameof(GroupMemberDetails.Email)}]                = member.Email, 
                                [{nameof(GroupMemberDetails.Pronouns)}]             = member.Pronouns, 
                                [{nameof(GroupMemberDetails.DateJoinedUtc)}]        = FORMAT(groupUser.ApprovedToJoinDateUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(GroupMemberDetails.LastLoginUtc)}]         = FORMAT(member.LastLoginDateUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(GroupMemberDetails.Role)}]                 = memberRoles.RoleName

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    JOIN        MembershipUser member 
                    ON          member.Id = groupUser.MembershipUser_Id
                    JOIN        MembershipRole memberRoles 
                    ON          memberRoles.Id = groupUser.MembershipRole_Id 
                    WHERE       groups.Slug = @Slug
                    AND         member.Id = @UserId
                    AND         groupUser.Approved = 1;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var member = await dbConnection.QueryFirstOrDefaultAsync<GroupMemberDetails>(query, new
            {
                UserId = userId,
                Slug = slug
            });

            return member;
        }

        public async Task<GroupSite> GetGroupSiteDataAsync(Guid groupId, CancellationToken cancellationToken)
        {
            const string query =
                    @$"SELECT 
                                [{nameof(GroupSite.Id)}],
                                [{nameof(GroupSite.GroupId)}],
                                [{nameof(GroupSite.ContentRootId)}]

                    FROM [GroupSite]
                    WHERE GroupId = @GroupId;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                GroupId = groupId
            });

            var groupSiteData = await reader.ReadFirstAsync<GroupSite>();

            return groupSiteData;
        }
    }

}
