using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Infrastructure.Models.GroupPages;

namespace FutureNHS.Api.DataAccess.Repositories.Read
{
    public class GroupDataProvider : IGroupDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<GroupDataProvider> _logger;

        public GroupDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<GroupDataProvider> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
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
                @"SELECT g.Id AS Id, g.Slug AS Slug, g.Name AS NameText, g.Description AS StrapLineText, 
				(SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = g.Id AND groupUser.Approved = 1 ) AS MemberCount, 
				(SELECT COUNT(*) FROM Topic topic WHERE topic.Group_Id = g.Id) AS DiscussionCount,
                image.Id, image.Height AS Height, image.Width AS Width, image.MediaType AS MediaType
				FROM [Group] g
                JOIN GroupUser groupUser ON groupUser.Group_Id = g.Id
                LEFT JOIN Image image ON image.Id = g.HeaderImage 
                WHERE groupUser.MembershipUser_Id = @UserId AND groupUser.Approved = 1 
                ORDER BY g.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) FROM GroupUser groupUser
                WHERE groupUser.MembershipUser_Id = @UserId AND groupUser.Approved = 1";
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
                            var groupWithImage = group with { Image = image };

                            return groupWithImage;
                        }

                        return group;

                    }, splitOn: "id");

                totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<uint>());
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
                @"SELECT g.Id AS Id, g.Slug AS Slug, g.Name AS NameText, g.Description AS StrapLineText, 
				(SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = g.Id AND groupUser.Approved = 1 ) AS MemberCount, 
				(SELECT COUNT(*) FROM Topic topic WHERE topic.Group_Id = g.Id) AS DiscussionCount,
                image.Id, image.Height AS Height, image.Width AS Width, image.MediaType AS MediaType
				FROM [Group] g    
                LEFT JOIN Image image ON image.Id = g.HeaderImage 
                WHERE NOT EXISTS (select gu.Group_Id from GroupUser gu where  gu.MembershipUser_Id = @UserId AND gu.Group_Id = g.Id AND gu.Approved = 1)
                ORDER BY g.Name
                OFFSET @Offset ROWS
                FETCH NEXT @Limit ROWS ONLY;

                SELECT COUNT(*) FROM [Group] g
                WHERE NOT EXISTS (select gu.Group_Id from GroupUser gu where  gu.MembershipUser_Id = @UserId AND gu.Group_Id = g.Id AND gu.Approved = 1)";

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
                            var groupWithImage = group with { Image = image };

                            return groupWithImage;
                        }

                        return group;

                    }, splitOn: "id");

                totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());
            }

            return (totalCount, groups);
        }

        public async Task<Group?> GetGroupAsync(string slug, CancellationToken cancellationToken = default)
        {
            const string query =
                @"SELECT g.Id AS Id, g.Slug AS Slug, g.Name AS Name, g.Description AS StrapLine, g.PublicGroup AS IsPublic,		
                image.Id, image.Height AS Height, image.Width AS Width, image.MediaType AS MediaType
				FROM [Group] g
                LEFT JOIN Image image ON image.Id = g.HeaderImage 
                WHERE g.Slug = @Slug AND g.IsDeleted = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryAsync<Group, Image, Group>(query,
                (group, image) =>
                {
                    if (image is not null)
                    {
                        var groupWithImage = @group with { Image = image };

                        return groupWithImage;
                    }

                    return @group;

                }, new
                {
                    Slug = slug
                }, splitOn: "id");

            var @group = reader.FirstOrDefault();

            return group;
        }
    }
}
