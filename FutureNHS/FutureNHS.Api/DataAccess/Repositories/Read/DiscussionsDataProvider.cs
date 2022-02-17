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
        public async Task<(uint total, IEnumerable<Discussion>?)> GetDiscussionsForGroupAsync(Guid? userId, string groupSlug, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query = 
                @$"SELECT
                                [{nameof(DiscussionData.Id)}]                   = discussion.Id,
                                [{nameof(DiscussionData.Title)}]                = discussion.Title, 
	                            [{nameof(DiscussionData.CreatedByThisUser)}]	= ( SELECT      CASE 
                                                                                    WHEN        discussion.CreatedBy = @UserId
                                                                                    THEN        CAST(1 as bit) 
                                                                                    ELSE        CAST(0 as bit) 
                                                                                    END
                                                                                  ),       
                                [{nameof(DiscussionData.CreatedAtUtc)}]         = FORMAT(discussion.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.CreatedById)}]          = discussion.CreatedBy,
                                [{nameof(DiscussionData.CreatedByName)}]        = createdByUser.FirstName + ' ' + createdByUser.Surname,
                                [{nameof(DiscussionData.CreatedBySlug)}]        = createdByUser.Slug,
                                [{nameof(DiscussionData.LastComment)}]			= latestComment.Id,
                                [{nameof(DiscussionData.LastCommentAtUtc)}]     = FORMAT(latestComment.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.LastCommenterId)}]      = latestComment.CreatedBy,
                                [{nameof(DiscussionData.LastCommenterName)}]    = lastCommentUser.FirstName + ' ' + lastCommentUser.Surname,
                                [{nameof(DiscussionData.LastCommenterSlug)}]    = lastCommentUser.Slug,
                                [{nameof(DiscussionData.IsSticky)}]				= discussion.IsSticky,
								[{nameof(DiscussionData.Views)}]				= discussion.Views,
								[{nameof(DiscussionData.TotalComments)}]		= (SELECT COUNT(*) FROM Comment WHERE Discussion_Id = discussion.Id )
                    
                    FROM        Discussion discussion
					JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id
                    LEFT JOIN   MembershipUser createdByUser 
                    ON          createdByUser.Id = discussion.CreatedBy

					LEFT JOIN   
                                (
					            SELECT TOP 1 *
					            FROM Comment 
					            ORDER BY CreatedAtUTC
					            ) 
                                latestComment ON latestComment.Discussion_Id = discussion.Id

                    LEFT JOIN   MembershipUser lastCommentUser 
                    ON          lastCommentUser.Id = latestComment.CreatedBy

                    WHERE       groups.Slug = @Slug
                    AND         groups.IsDeleted = 0       
                    ORDER BY    discussion.CreatedAtUTC

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 

                    FROM        Discussion discussion
					JOIN        [Group] groups
                    ON          groups.Id = discussion.Group_Id
					WHERE       groups.Slug = @Slug
                    AND         groups.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = groupSlug,
                UserId = userId
            });

            var contents = await reader.ReadAsync<DiscussionData>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, GenerateDiscussionModelFromData(contents));
        }
        

        public async Task<Discussion?> GetDiscussionAsync(Guid? userId, string groupSlug, Guid id, CancellationToken cancellationToken)
        {
            const string query =
                @$" SELECT
                                [{nameof(DiscussionData.Id)}]                   = discussion.Id,
                                [{nameof(DiscussionData.Title)}]                = discussion.Title, 
								[{nameof(DiscussionData.Description)}]          = discussion.Content,
	                            [{nameof(DiscussionData.CreatedByThisUser)}]	= ( SELECT      CASE 
                                                                                    WHEN        discussion.CreatedBy = @UserId 
                                                                                    THEN        CAST(1 as bit) 
                                                                                    ELSE        CAST(0 as bit) 
                                                                                    END
                                                                                  ),   
                                [{nameof(DiscussionData.CreatedAtUtc)}]         = FORMAT(discussion.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.CreatedById)}]          = discussion.CreatedBy,
                                [{nameof(DiscussionData.CreatedByName)}]        = createdByUser.FirstName + ' ' + createdByUser.Surname,
                                [{nameof(DiscussionData.CreatedBySlug)}]        = createdByUser.Slug,
                                [{nameof(DiscussionData.LastComment)}]			= latestComment.Id,
                                [{nameof(DiscussionData.LastCommentAtUtc)}]     = FORMAT(latestComment.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.LastCommenterId)}]      = latestComment.CreatedBy,
                                [{nameof(DiscussionData.LastCommenterName)}]    = lastCommentUser.FirstName + ' ' + lastCommentUser.Surname,
                                [{nameof(DiscussionData.LastCommenterSlug)}]    = lastCommentUser.Slug,
                                [{nameof(DiscussionData.IsSticky)}]				= discussion.IsSticky,
								[{nameof(DiscussionData.Views)}]				= discussion.Views,
								[{nameof(DiscussionData.TotalComments)}]		= (SELECT COUNT(*) FROM Comment WHERE Discussion_Id = discussion.Id)
                    
                    FROM        Discussion discussion
                    JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id

                    LEFT JOIN   MembershipUser createdByUser 
                    ON          createdByUser.Id = discussion.CreatedBy
          
					LEFT JOIN   
                                (
					            SELECT TOP 1 *
					            FROM Comment 
					            ORDER BY CreatedAtUTC
					            ) 
                                latestComment ON latestComment.Discussion_Id = discussion.Id

                    LEFT JOIN   MembershipUser lastCommentUser 
                    ON          lastCommentUser.Id = latestComment.CreatedBy
                    
                    WHERE       discussion.Id = @Id 
                    AND         groups.Slug = @Slug
                    AND         groups.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryAsync<DiscussionData>(query, new
            {
                Slug = groupSlug,
                Id = id,
                UserId = userId
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
                    Description = item.Description,
                    IsSticky = item.IsSticky,
                    Views = item.Views,
                    TotalComments = item.TotalComments,
                    CurrentUser = new UserDiscussionDetails
                    {
                        Created = item.CreatedByThisUser
                    },
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
