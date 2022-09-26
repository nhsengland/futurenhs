using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Models.User;
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
				(SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = g.Id AND discussion.IsDeleted = 0) AS DiscussionCount,
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
				(SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = g.Id AND discussion.IsDeleted = 0) AS DiscussionCount,
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

        public async Task<(uint totalGroups, IEnumerable<AdminGroupSummary> groupSummaries)> AdminGetGroupsAsync(uint offset, uint limit, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            uint totalCount;

            IEnumerable<AdminGroupSummary> groups;

            const string query =
                @"SELECT g.Id AS Id, g.ThemeId AS ThemeId, g.Slug AS Slug, g.Name AS NameText, g.Subtitle AS StrapLineText,
				(SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = g.Id AND groupUser.Approved = 1 ) AS MemberCount, 
				(SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = g.Id AND discussion.IsDeleted = 0) AS DiscussionCount,
                image.Id, image.Height AS Height, image.Width AS Width, image.FileName AS FileName, image.MediaType AS MediaType,
                owner.Id, owner.FirstName + ' ' + owner.Surname AS Name, owner.Slug AS Slug
				FROM [Group] g
                LEFT JOIN Image image ON image.Id = g.ImageId
                LEFT JOIN MembershipUser owner ON owner.Id = g.GroupOwner
                WHERE g.IsDeleted = 0
                ORDER BY g.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) 
                FROM [Group] g
                WHERE g.IsDeleted = 0";
            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                using var reader = await dbConnection.QueryMultipleAsync(query, new
                {
                    Offset = Convert.ToInt32(offset),
                    Limit = Convert.ToInt32(limit)
                });
                groups = reader.Read<AdminGroupSummary, ImageData, UserNavProperty, AdminGroupSummary>(
                    (group, image, owner) =>
                    {
                        if (image is not null)
                        {
                            var groupWithImage = group with { Image = new ImageData(image, _options) };

                            if (owner is not null)
                            {
                                var groupWithOwner = groupWithImage with { Owner = owner };

                                return groupWithOwner;
                            }

                            return groupWithImage;
                        }

                        if (owner is not null)
                        {
                            var groupWithOwner = group with { Owner = owner };

                            return groupWithOwner;
                        }

                        return group;

                    }, splitOn: "id");

                totalCount = await reader.ReadFirstAsync<uint>();
            }

            return (totalCount, groups);
        }

        public async Task<Group?> GetGroupAsync(string slug, Guid userId, CancellationToken cancellationToken = default)
        {
            const string query =
                @"SELECT g.Id AS Id, g.ThemeId AS ThemeId, g.Slug AS Slug, g.Name AS Name, g.Subtitle AS Strapline, g.PublicGroup AS IsPublic,( SELECT CASE 
                                                                                    WHEN        gu.Approved = 1
                                                                                    AND         gu.Rejected = 0
                                                                                    AND         gu.Locked = 0
                                                                                    AND         gu.Banned = 0
                                                                                    THEN        'Approved'
                                                                                    WHEN        gu.Approved = 0
                                                                                    AND         gu.Rejected = 0
                                                                                    AND         gu.Locked = 0
                                                                                    AND         gu.Banned = 0
                                                                                    THEN        'Pending Approval'
                                                                                    ELSE        'Non Member'
                                                                                    END
                                                                                  ) AS MemberStatus,		
                image.Id, image.Height AS Height, image.Width AS Width, image.FileName AS FileName,  image.MediaType AS MediaType
				FROM [Group] g
                LEFT JOIN [Image] image ON image.Id = g.ImageId  
                LEFT JOIN GroupUser gu ON (gu.Group_Id = g.Id and gu.MembershipUser_Id = @UserId)
                WHERE g.Slug = @Slug AND g.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var group = await dbConnection.QueryAsync<Group, Image, Group>(query,
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

            return group.SingleOrDefault() ?? throw new NotFoundException("Group not found.");
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
                                [{nameof(GroupMember.GroupUserId)}]          = groupUser.Id,
                                [{nameof(GroupMember.Slug)}]                 = member.Slug, 
                                [{nameof(GroupMember.Name)}]                 = member.FirstName + ' ' +  member.Surname, 
                                [{nameof(GroupMember.DateJoinedUtc)}]        = groupUser.ApprovedToJoinDateUTC,
                                [{nameof(GroupMember.LastLoginUtc)}]         = member.LastLoginDateUTC,
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
                    AND         member.IsDeleted = 0
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
                                [{nameof(PendingGroupMember.ApplicationDateUtc)}]   = groupUser.RequestToJoinDateUTC,
                                [{nameof(PendingGroupMember.LastLoginUtc)}]         = member.LastLoginDateUTC,
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
                    AND         member.IsDeleted = 0
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
                                [{nameof(GroupMemberDetails.DateJoinedUtc)}]        = groupUser.ApprovedToJoinDateUTC,
                                [{nameof(GroupMemberDetails.LastLoginUtc)}]         = member.LastLoginDateUTC,
                                [{nameof(GroupMemberDetails.Role)}]                 = memberRoles.RoleName,
                                [{nameof(GroupMemberDetails.RoleId)}]               = groupUser.MembershipRole_Id,
                                [{nameof(ImageData.Id)}]		                    = [image].Id,
                                [{nameof(ImageData.Height)}]	                    = [image].Height,
                                [{nameof(ImageData.Width)}]		                    = [image].Width,
                                [{nameof(ImageData.FileName)}]	                    = [image].FileName,
                                [{nameof(ImageData.MediaType)}]	                    = [image].MediaType 


                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    JOIN        MembershipUser member 
                    ON          member.Id = groupUser.MembershipUser_Id
                    JOIN        MembershipRole memberRoles 
                    ON          memberRoles.Id = groupUser.MembershipRole_Id 
                    LEFT JOIN   Image [image]
                    ON          [image].Id = member.ImageId   
                    WHERE       groups.Slug = @Slug
                    AND         member.Id = @UserId
                    AND         groupUser.Approved = 1
                    AND         member.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var groupMemberDetails = await dbConnection.QueryAsync<GroupMemberDetails, Image, GroupMemberDetails>(query,
                            (member, image) =>
                            {
                                if (image is not null)
                                {
                                    return member with { Image = new ImageData(image, _options) };
                                }

                                return @member;
                            }, new
                            {
                                UserId = userId,
                                Slug = slug
                            });

            return groupMemberDetails.SingleOrDefault() ?? throw new NotFoundException("Group member details not found.");
        }


        public async Task<IEnumerable<GroupMemberDetails>> GetGroupAdminsAsync(string groupSlug, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(GroupMemberDetails.Id)}]                   = member.Id,
                                [{nameof(GroupMemberDetails.Slug)}]                 = member.Slug, 
                                [{nameof(GroupMemberDetails.FirstName)}]            = member.FirstName,
                                [{nameof(GroupMemberDetails.LastName)}]             = member.Surname,
                                [{nameof(GroupMemberDetails.Email)}]                = member.Email, 
                                [{nameof(GroupMemberDetails.Pronouns)}]             = member.Pronouns, 
                                [{nameof(GroupMemberDetails.DateJoinedUtc)}]        = groupUser.ApprovedToJoinDateUTC,
                                [{nameof(GroupMemberDetails.LastLoginUtc)}]         = member.LastLoginDateUTC,
                                [{nameof(GroupMemberDetails.Role)}]                 = memberRoles.RoleName,
                                [{nameof(GroupMemberDetails.RoleId)}]               = groupUser.MembershipRole_Id

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    JOIN        MembershipUser member 
                    ON          member.Id = groupUser.MembershipUser_Id
                    JOIN        MembershipRole memberRoles 
                    ON          memberRoles.Id = groupUser.MembershipRole_Id 
                    WHERE       groups.Slug = @GroupSlug
                    AND         member.IsDeleted = 0
                    AND         groupUser.MembershipRole_Id = (SELECT Id FROM MembershipRole WHERE RoleName = 'Admin')";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var member = await dbConnection.QueryAsync<GroupMemberDetails>(query, new
            {
                GroupSlug = groupSlug
            });

            return member;
        }

        public async Task<GroupSite> GetGroupSiteDataAsync(string groupSlug, CancellationToken cancellationToken)
        {
            const string query =
                    @$"SELECT 
                                gs.Id,
                                gs.GroupId,
                                gs.ContentRootId,
								g.Slug

                    FROM [GroupSite] gs
					JOIN        [Group] g
                    ON          g.Id = gs.GroupId
					WHERE Slug = @GroupSlug;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                GroupSlug = groupSlug
            });

            var groupSiteData = await reader.ReadSingleOrDefaultAsync<GroupSite>();

            return groupSiteData;
        }

        public async Task<bool> GetGroupPrivacyStatusAsync(string groupSlug, CancellationToken cancellationToken = default)
        {
            const string query =
                             @$"SELECT
                                    PublicGroup
                               FROM[Group]
                               WHERE Slug = @Slug";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var isPublicGroup = await dbConnection.QuerySingleOrDefaultAsync<bool>(query, new
            {
                Slug = groupSlug
            });

            return isPublicGroup;
        }

        public async Task<bool> GetGroupPrivacyStatusAsync(Guid GroupId, CancellationToken cancellationToken = default)
        {
            const string query =
                            @$"SELECT 
                                    PublicGroup
                               FROM [Group] 
                               WHERE Id = @Id";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var isPublicGroup = await dbConnection.QuerySingleOrDefaultAsync<bool>(query, new
            {
                Id = GroupId
            });

            return isPublicGroup;
        }
    }
}
