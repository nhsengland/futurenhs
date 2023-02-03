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

        public async Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid userId, bool isMember, uint offset, uint limit, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            uint totalCount;

            IEnumerable<GroupSummary> groups;

            var isMemberQuery = isMember == true
                ? "JOIN GroupUser groupUser ON groupUser.Group_Id = groups.Id WHERE groups.IsDeleted = 0 AND groupUser.MembershipUser_Id = @UserId AND groupUser.Approved = 1"
                : "WHERE groups.IsDeleted = 0 AND NOT EXISTS (select gu.Group_Id from GroupUser gu where  gu.MembershipUser_Id = @UserId AND gu.Group_Id = groups.Id AND gu.Approved = 1)";
            
            string query =
                @$"SELECT 
                    [{nameof(GroupSummary.Id)}]                        = groups.Id,
                    [{nameof(GroupSummary.ThemeId)}]                   = groups.ThemeId,
                    [{nameof(GroupSummary.Slug)}]                      = groups.Slug,
                    [{nameof(GroupSummary.NameText)}]                  = groups.Name,
                    [{nameof(GroupSummary.StraplineText)}]             = groups.Subtitle,
                    [{nameof(GroupSummary.IsPublic)}]                  = groups.IsPublic,
                    [{nameof(GroupSummary.MemberCount)}]               = (SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = groups.Id AND groupUser.Approved = 1 ), 
				    [{nameof(GroupSummary.DiscussionCount)}]           = (SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = groups.Id AND discussion.IsDeleted = 0),
                    [{nameof(GroupSummary.FileCount)}]                 = (SELECT COUNT(*) FROM [File] files JOIN Folder folder on folder.Id = files.ParentFolder WHERE folder.Group_Id = groups.Id AND files.IsDeleted = 0),
                    [{nameof(ImageData.Id)}]		                   = image.Id,
                    [{nameof(ImageData.Height)}]	                   = image.Height,
                    [{nameof(ImageData.Width)}]		                   = image.Width,
                    [{nameof(ImageData.FileName)}]	                   = image.FileName,
                    [{nameof(ImageData.MediaType)}]	                   = image.MediaType
				FROM [Group] groups
                LEFT JOIN Image image ON image.Id = groups.ImageId                         
                {isMemberQuery}
                ORDER BY groups.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) FROM [Group] groups
                {isMemberQuery}";
            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                using var reader = await dbConnection.QueryMultipleAsync(query, new
                {
                    Offset = Convert.ToInt32(offset),
                    Limit = Convert.ToInt32(limit),
                    UserId = userId
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

        public async Task<(uint totalGroups, IEnumerable<GroupInviteSummary> groupSummaries)> GetGroupInvitesForUserAsync(Guid userId, IEnumerable<GroupInvite> groupInvites,
            uint offset, uint limit, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            uint totalCount;

            IEnumerable<GroupInviteSummary> groups;

            var invitesList = groupInvites.ToList();
            var groupInviteIdArray = invitesList.Any() ? String.Join(",", invitesList.Select(invite => $"'{invite.GroupId}'")) : $"'{default(Guid)}'";
            var summaryQuery = $"WHERE groups.IsDeleted = 0 AND groups.Id IN ({groupInviteIdArray})";

            string query =
                @$"SELECT 
                    [{nameof(GroupInviteSummary.Id)}]                  = groups.Id,
                    [{nameof(GroupInviteSummary.ThemeId)}]             = groups.ThemeId,
                    [{nameof(GroupInviteSummary.Slug)}]                = groups.Slug,
                    [{nameof(GroupInviteSummary.NameText)}]            = groups.Name,
                    [{nameof(GroupInviteSummary.StraplineText)}]       = groups.Subtitle,
                    [{nameof(GroupInviteSummary.IsPublic)}]            = groups.IsPublic,
                    [{nameof(GroupInviteSummary.MemberCount)}]         = (SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = groups.Id AND groupUser.Approved = 1 ), 
				    [{nameof(GroupInviteSummary.DiscussionCount)}]     = (SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = groups.Id AND discussion.IsDeleted = 0),            
                    [{nameof(GroupSummary.FileCount)}]                 = (SELECT COUNT(*) FROM [File] files JOIN Folder folder on folder.Id = files.ParentFolder WHERE folder.Group_Id = groups.Id AND files.IsDeleted = 0),
                    [{nameof(ImageData.Id)}]		                   = image.Id,
                    [{nameof(ImageData.Height)}]	                   = image.Height,
                    [{nameof(ImageData.Width)}]		                   = image.Width,
                    [{nameof(ImageData.FileName)}]	                   = image.FileName,
                    [{nameof(ImageData.MediaType)}]	                   = image.MediaType
				FROM [Group] groups
                LEFT JOIN Image image ON image.Id = groups.ImageId
                {summaryQuery}
                ORDER BY groups.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) FROM [Group] groups
                {summaryQuery}";
            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                using var reader = await dbConnection.QueryMultipleAsync(query, new
                {
                    Offset = Convert.ToInt32(offset),
                    Limit = Convert.ToInt32(limit),
                });
                groups = reader.Read<GroupInviteSummary, ImageData, GroupInviteSummary>(
                    (group, image) =>
                    {
                        var invite = invitesList.Single(gi => gi.GroupId.Equals(group.Id));
                        if (invite is not null)
                        {
                            var groupWithInvite = group with { Invite = new GroupInvite(invite) };
                            if (image is not null)
                            {
                            
                                var groupWithImage = groupWithInvite with { Image = new ImageData(image, _options) };
                                return groupWithImage;
                            }
                            return groupWithInvite;
                        }
                        return group;
                    }, splitOn: "id");

                totalCount = await reader.ReadFirstAsync<uint>();
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
                @$"SELECT 
                    [{nameof(AdminGroupSummary.Id)}]                   = groups.Id,
                    [{nameof(AdminGroupSummary.ThemeId)}]              = groups.ThemeId,
                    [{nameof(AdminGroupSummary.Slug)}]                 = groups.Slug,
                    [{nameof(AdminGroupSummary.NameText)}]             = groups.Name,
                    [{nameof(AdminGroupSummary.StrapLineText)}]        = groups.Subtitle,
                    [{nameof(AdminGroupSummary.IsPublic)}]             = groups.IsPublic,
                    [{nameof(AdminGroupSummary.MemberCount)}]          = (SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = groups.Id AND groupUser.Approved = 1 ), 
				    [{nameof(AdminGroupSummary.DiscussionCount)}]      = (SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = groups.Id AND discussion.IsDeleted = 0),
                    [{nameof(ImageData.Id)}]		                   = image.Id,
                    [{nameof(ImageData.Height)}]	                   = image.Height,
                    [{nameof(ImageData.Width)}]		                   = image.Width,
                    [{nameof(ImageData.FileName)}]	                   = image.FileName,
                    [{nameof(ImageData.MediaType)}]	                   = image.MediaType,  
                    [{nameof(UserNavProperty.Id)}]	                   = owner.Id,  
                    [{nameof(UserNavProperty.Name)}]	               = owner.FirstName + ' ' + owner.Surname,
                    [{nameof(UserNavProperty.Slug)}]	               = owner.Slug
				FROM [Group] groups 
                LEFT JOIN Image image ON image.Id = groups.ImageId
                LEFT JOIN MembershipUser owner ON owner.Id = groups.GroupOwner
                WHERE groups.IsDeleted = 0
                ORDER BY groups.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) 
                FROM [Group] groups
                WHERE groups.IsDeleted = 0";
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
                @$"SELECT 
                    [{nameof(Group.Id)}]                   = groups.Id,
                    [{nameof(Group.ThemeId)}]              = groups.ThemeId,
                    [{nameof(Group.Slug)}]                 = groups.Slug,
                    [{nameof(Group.Name)}]                 = groups.Name,
                    [{nameof(Group.Strapline)}]            = groups.Subtitle,
                    [{nameof(Group.IsPublic)}]             = groups.IsPublic,
                    [{nameof(Group.MemberStatus)}]         = ( SELECT CASE 
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
                                                                        END),
                    [{nameof(ImageData.Id)}]		        = image.Id,
                    [{nameof(ImageData.Height)}]	        = image.Height,
                    [{nameof(ImageData.Width)}]		        = image.Width,
                    [{nameof(ImageData.FileName)}]	        = image.FileName,
                    [{nameof(ImageData.MediaType)}]	        = image.MediaType
				FROM [Group] groups
                LEFT JOIN [Image] image ON image.Id = groups.ImageId  
                LEFT JOIN GroupUser gu ON (gu.Group_Id = groups.Id and gu.MembershipUser_Id = @UserId)
                WHERE groups.Slug = @Slug AND groups.IsDeleted = 0;";

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
                                [{nameof(GroupMember.LastLoginUtc)}]         = memberactivity.LastActivityDateUTC,
                                [{nameof(GroupMember.Role)}]                 = memberRoles.RoleName

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    JOIN        MembershipUser member 
                    ON          member.Id = groupUser.MembershipUser_Id
                    JOIN        MembershipRole memberRoles 
                    ON          memberRoles.Id = groupUser.MembershipRole_Id 
                    LEFT JOIN   MembershipUserActivity memberactivity 
                    ON          memberactivity.MembershipUserId = member.Id
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
                                [{nameof(GroupMemberDetails.LastLoginUtc)}]         = memberactivity.LastActivityDateUTC,
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
                    LEFT JOIN   MembershipUserActivity memberactivity 
                    ON          memberactivity.MembershipUserId = member.Id
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
                                [{nameof(GroupMemberDetails.LastLoginUtc)}]         = memberactivity.LastActivityDateUTC,
                                [{nameof(GroupMemberDetails.Role)}]                 = memberRoles.RoleName,
                                [{nameof(GroupMemberDetails.RoleId)}]               = groupUser.MembershipRole_Id

                    FROM        GroupUser groupUser
                    JOIN        [Group] groups 
                    ON          groups.Id = groupUser.Group_Id
                    JOIN        MembershipUser member 
                    ON          member.Id = groupUser.MembershipUser_Id
                    JOIN        MembershipRole memberRoles 
                    ON          memberRoles.Id = groupUser.MembershipRole_Id 
                    LEFT JOIN   MembershipUserActivity memberactivity 
                    ON          memberactivity.MembershipUserId = member.Id
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
                                    IsPublic
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
                                    IsPublic
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
