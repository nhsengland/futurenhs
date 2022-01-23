using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.DataAccess.Models.Discussions;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.Read
{
    public class DiscussionDataProvider : IDiscussionDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<DiscussionDataProvider> _logger;

        public DiscussionDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<DiscussionDataProvider> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }
        public async Task<(uint total, IEnumerable<Discussion>?)> GetDiscussionsForGroupAsync(string groupSlug, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query = 
                @$" SELECT
                                [{nameof(DiscussionData.Id)}]                   = discussion.Id,
                                [{nameof(DiscussionData.Title)}]                = discussion.Name, 
								[{nameof(DiscussionData.Slug)}]                 = discussion.Slug, 
                                [{nameof(DiscussionData.CreatedAtUtc)}]         = FORMAT(discussion.CreateDate,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.CreatedById)}]          = discussion.MembershipUser_Id,
                                [{nameof(DiscussionData.CreatedByName)}]        = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(DiscussionData.CreatedBySlug)}]        = createUser.Slug,
                                [{nameof(DiscussionData.LastComment)}]			= comment.Id,
                                [{nameof(DiscussionData.LastCommentAtUtc)}]     = FORMAT(comment.DateCreated,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.LastCommenterId)}]      = comment.MembershipUser_Id,
                                [{nameof(DiscussionData.LastCommenterName)}]    = lastCommentUser.FirstName + ' ' + lastCommentUser.Surname,
                                [{nameof(DiscussionData.LastCommenterSlug)}]    = lastCommentUser.Slug,
                                [{nameof(DiscussionData.IsSticky)}]				= discussion.IsSticky,
								[{nameof(DiscussionData.Views)}]				= discussion.Views,
								[{nameof(DiscussionData.TotalComments)}]		= (SELECT COUNT(*) FROM Post WHERE Topic_Id = discussion.Id and IsTopicStarter = 0 )
                    
                    FROM        Topic discussion
					JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id
                    LEFT JOIN   MembershipUser createUser 
                    ON          CreateUser.Id = discussion.MembershipUser_Id
                    JOIN        Post comment 
                    ON          comment.Id = discussion.Post_Id
					LEFT JOIN   MembershipUser lastCommentUser 
                    ON          lastCommentUser.Id = discussion.MembershipUser_Id
                    WHERE       groups.Slug = @Slug 
                    AND         groups.IsDeleted = 0       
                    ORDER BY    comment.DateCreated

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 

                    FROM        Topic discussion
					JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id
					WHERE       groups.Slug = @Slug 
                    AND         groups.IsDeleted = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = groupSlug
            });

            var contents = await reader.ReadAsync<DiscussionData>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, GenerateDiscussionModelFromData(contents));
        }
        

        public async Task<Discussion?> GetDiscussionAsync(Guid id, string groupSlug, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT
                                [{nameof(DiscussionData.Id)}]                   = discussion.Id,
                                [{nameof(DiscussionData.Title)}]                = discussion.Name, 
								[{nameof(DiscussionData.Slug)}]                 = discussion.Slug, 
								[{nameof(DiscussionData.Description)}]          = comment.PostContent, 
                                [{nameof(DiscussionData.CreatedAtUtc)}]         = FORMAT(discussion.CreateDate,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.CreatedById)}]          = discussion.MembershipUser_Id,
                                [{nameof(DiscussionData.CreatedByName)}]        = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(DiscussionData.CreatedBySlug)}]        = createUser.Slug,
                                [{nameof(DiscussionData.LastComment)}]			= comment.Id,
                                [{nameof(DiscussionData.LastCommentAtUtc)}]     = FORMAT(comment.DateCreated,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.LastCommenterId)}]      = comment.MembershipUser_Id,
                                [{nameof(DiscussionData.LastCommenterName)}]    = lastCommentUser.FirstName + ' ' + lastCommentUser.Surname,
                                [{nameof(DiscussionData.LastCommenterSlug)}]    = lastCommentUser.Slug,
                                [{nameof(DiscussionData.IsSticky)}]				= discussion.IsSticky,
								[{nameof(DiscussionData.Views)}]				= discussion.Views,
								[{nameof(DiscussionData.TotalComments)}]		= (SELECT COUNT(*) FROM Post WHERE Topic_Id = discussion.Id and IsTopicStarter = 0 )
                    
                    FROM        Topic discussion
                    JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id
                    LEFT JOIN   MembershipUser createUser 
                    ON          CreateUser.Id = discussion.MembershipUser_Id
                    JOIN        Post comment 
                    ON          comment.Id = discussion.Post_Id
					LEFT JOIN   MembershipUser lastCommentUser 
                    ON          lastCommentUser.Id = discussion.MembershipUser_Id

                    WHERE       discussion.Id = @Id 
                    AND         groups.Slug = @GroupSlug";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryAsync<DiscussionData>(query, new
            {
                GroupSlug = groupSlug,
                Id = id
            });
            
            return GenerateDiscussionModelFromData(reader).FirstOrDefault();
        }

        private IEnumerable<Discussion> GenerateDiscussionModelFromData(IEnumerable<DiscussionData> discussionData)
        {
            var contentItems = new List<Discussion>();

            foreach (var item in discussionData)
            {
                var discussion = new Discussion()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Slug = item.Slug,
                    Description = item.Description,
                    IsSticky = item.IsSticky,
                    Views = item.Views,
                    TotalComments = item.TotalComments,
                    FirstRegistered = new Models.Shared.Properties
                    {
                        AtUtc = item.CreatedAtUtc,
                        By = new UserNavProperty
                        {
                            Id = item.CreatedById,
                            Name = item.CreatedByName,
                            Slug = item.CreatedBySlug
                        }
                    }
                };

                if (item.LastComment is not null)
                {
                    discussion = discussion with
                    {
                        LastComment = new CommentNavProperty
                        {
                            AtUtc = item.LastCommentAtUtc,
                            Id = item.LastComment,
                            By = new UserNavProperty
                            {
                                Id = item.LastCommenterId.GetValueOrDefault(),
                                Name = item.LastCommenterName,
                                Slug = item.LastCommenterSlug
                            }
                        }
                    };
                }

                contentItems.Add(discussion);
            }

            return contentItems;
        }
    }
}
