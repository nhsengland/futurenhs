using Dapper;
using FutureNHS.Api.DataAccess.Models.GroupPages;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Application.Application.HardCodedSettings;
using FutureNHS.Application.Interfaces;
using FutureNHS.Infrastructure.Models;
using FutureNHS.Infrastructure.Models.GroupPages;
using FutureNHS.Infrastructure.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Infrastructure.Repositories.Read.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.Read
{
    public class GroupDataProvider : IGroupDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;

        public GroupDataProvider(IAzureSqlDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<(int totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid id, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < PaginationSettings.MinPageNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            if (pageSize is < PaginationSettings.MinPageSize or > PaginationSettings.MaxPageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            int totalCount;

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
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*) FROM GroupUser groupUser
                WHERE groupUser.MembershipUser_Id = @UserId AND groupUser.Approved = 1";
            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                using var reader = await dbConnection.QueryMultipleAsync(query, new
                {
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize,
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

                totalCount = await reader.ReadFirstAsync<int>();
            }

            return (totalCount, groups);
        }

        public async Task<(int totalGroups, IEnumerable<GroupSummary> groupSummaries)> DiscoverGroupsForUserAsync(Guid id, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < PaginationSettings.MinPageNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            if (pageSize is < PaginationSettings.MinPageSize or > PaginationSettings.MaxPageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            int totalCount;

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
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*) FROM [Group] g
                WHERE NOT EXISTS (select gu.Group_Id from GroupUser gu where  gu.MembershipUser_Id = @UserId AND gu.Group_Id = g.Id AND gu.Approved = 1)";

            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                using var reader = await dbConnection.QueryMultipleAsync(query, new
                {
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize,
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

                totalCount = await reader.ReadFirstAsync<int>();
            }

            return (totalCount, groups);
        }

        public async Task<GroupHeader> GetGroupHeaderForUserAsync(Guid userId, string slug, CancellationToken cancellationToken = default)
        {
            GroupHeader group;

            const string query =
                @"SELECT g.Id AS Id, g.Slug AS Slug, g.Name AS NameText, g.Description AS StrapLineText, GroupUser.Approved as UserApproved,		
                image.Id, image.Height AS Height, image.Width AS Width, image.MediaType AS MediaType
				FROM [Group] g
                LEFT JOIN GroupUser groupUser ON groupUser.Group_Id = g.Id AND GroupUser.MembershipUser_Id = @UserId
                LEFT JOIN Image image ON image.Id = g.HeaderImage 
                WHERE g.Slug = @Slug";

            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                var reader = await dbConnection.QueryAsync<GroupHeader,Image,GroupHeader>(query, 
                (group, image) =>
                    {
                        if (image is not null)
                        {
                            var groupWithImage = group with { Image = image };

                            return groupWithImage;
                        }

                        return group;

                    }, new
                    {
                        UserId = userId,
                        Slug = slug
                    }, splitOn: "id");

                group = reader.First();
            }

            return group;
        }


        public async Task<GroupHomePage> GetGroupHomePage(string slug, CancellationToken cancellationToken = default)
        {
            GroupHomePage groupHomePage;

            const string query =
                @"SELECT g.Subtitle AS SubtitleText, g.Introduction AS BodyHtml
                FROM [Group] g
                WHERE g.Slug = @Slug";

            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                groupHomePage = await dbConnection.QuerySingleAsync<GroupHomePage>(query,new
                {
                    Slug = slug
                });
            }

            return groupHomePage;
        }
    }
}
