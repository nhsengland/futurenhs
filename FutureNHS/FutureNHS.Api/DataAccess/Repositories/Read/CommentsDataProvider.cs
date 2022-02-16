using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.DataAccess.Models.Discussions;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.Read
{
    public class CommentsDataProvider : ICommentsDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<CommentsDataProvider> _logger;

        public CommentsDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<CommentsDataProvider> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }
        public async Task<(uint total, IEnumerable<Comment>?)> GetCommentsForDiscussionAsync(Guid? userId, string groupSlug, Guid topicId, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @$"SELECT
                                [{nameof(CommentData.Id)}]                  = comment.Id,
                                [{nameof(CommentData.Content)}]             = comment.Content, 
	                            [{nameof(CommentData.CreatedByThisUser)}]	= ( SELECT      CASE 
                                                                                WHEN        comment.CreatedBy = @UserId 
                                                                                THEN        CAST(1 as bit) 
                                                                                ELSE        CAST(0 as bit) 
                                                                                END
                                                                              ),               
                                [{nameof(CommentData.CreatedAtUtc)}]        = FORMAT(comment.CreatedAtUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(CommentData.CreatedById)}]         = comment.CreatedBy,
                                [{nameof(CommentData.CreatedByName)}]       = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(CommentData.CreatedBySlug)}]       = createUser.Slug,
								[{nameof(CommentData.RepliesCount)}]        = ( SELECT      COUNT(*) 
                                                                                FROM        Comment replies 
                                                                                WHERE       replies.ThreadId = post.Id
                                                                              ),
								[{nameof(CommentData.Likes)}]				= ( SELECT      COUNT(*) 
                                                                                FROM        [Like] 
                                                                                WHERE       Comment_Id = comment.Id
                                                                              ),
								[{nameof(CommentData.LikedByThisUser)}]		= ( SELECT      CASE 
                                                                                WHEN        Id IS NULL 
                                                                                THEN        CAST(0 as bit) 
                                                                                ELSE        CAST(1 as bit) 
                                                                                END 
                                                                                FROM        [Like]  
                                                                                WHERE       [Like].Comment_Id = comment.Id 
                                                                                AND         [Like].CreatedBy = @UserId
                                                                              )

                    FROM            Comment comment
					Join		    Discussion discussion
					ON			    discussion.Id = comment.Discussion_Id
					JOIN		    [Group] groups
					ON			    groups.Id = discussion.Group_Id
                    LEFT JOIN       MembershipUser createUser 
                    ON              CreateUser.Id = comment.CreatedBy		
					WHERE           comment.Discussion_Id = @DiscussionId 
                    AND             comment.ThreadId IS NULL
                    AND             groups.Slug = @Slug
                    ORDER BY        comment.CreatedAtUTC

                    OFFSET          @Offset ROWS
                    FETCH NEXT      @Limit ROWS ONLY;

                    SELECT          COUNT(*) 

                    FROM            Comment comment		
					Join		    Discussion discussion
					ON			    discussion.Id = comment.Discussion_Id
					JOIN		    [Group] groups
					ON			    groups.Id = discussion.Group_Id
					WHERE           comment.Discussion_Id = @DiscussionId 
                    AND             comment.ThreadId IS NULL
                    AND             groups.Slug = @Slug";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = groupSlug,
                DiscussionId = topicId,
                UserId = userId
            });

            var commentsData = await reader.ReadAsync<CommentData>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, GenerateCommentModelFromData(commentsData));
        }


        public async Task<(uint total, IEnumerable<Comment>?)> GetRepliesForCommentAsync(Guid? userId, string groupSlug, Guid threadId, uint offset, uint limit, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT
                                [{nameof(CommentData.Id)}]                  = comment.Id,
                                [{nameof(CommentData.Content)}]             = comment.Content, 
	                            [{nameof(CommentData.CreatedByThisUser)}]	= ( SELECT      CASE 
                                                                                WHEN        comment.CreatedBy = @UserId
                                                                                THEN        CAST(1 as bit) 
                                                                                ELSE        CAST(0 as bit) 
                                                                                END
                                                                              ),               
                                [{nameof(CommentData.CreatedAtUtc)}]        = FORMAT(comment.CreatedAtUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(CommentData.CreatedById)}]         = comment.MembershipUser_Id,
                                [{nameof(CommentData.CreatedByName)}]       = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(CommentData.CreatedBySlug)}]       = createUser.Slug,
								[{nameof(CommentData.Likes)}]				= ( SELECT      COUNT(*) 
                                                                                FROM        Like 
                                                                                WHERE       Comment_Id = comment.Id
                                                                              ),
								[{nameof(CommentData.LikedByThisUser)}]		= ( SELECT      CASE 
                                                                                WHEN        Id IS NULL 
                                                                                THEN        CAST(0 as bit) 
                                                                                ELSE        CAST(1 as bit) 
                                                                                END 
                                                                                FROM        Like   
                                                                                WHERE       Like.Comment_Id = post.Id 
                                                                                AND         Like.CreatedBy = @UserId
                                                                              )

                    FROM            Comment comment
					JOIN		    Discussion discussion
					ON			    discussion.Id = comment.Discussion_Id
					JOIN		    [Group] groups
					ON			    groups.Id = discussion.Group_Id
                    LEFT JOIN       MembershipUser createUser 
                    ON              CreateUser.Id = comment.CreatedBy		
					WHERE           comment.ThreadId = @ThreadId 
                    AND             groups.Slug = @Slug
                    ORDER BY        comment.CreatedAtUTC

                    OFFSET          @Offset ROWS
                    FETCH NEXT      @Limit ROWS ONLY;

                    SELECT          COUNT(*) 

                    FROM            Comment comment
					JOIN		    Discussion discussion
					ON			    discussion.Id = comment.Topic_Id
					JOIN		    [Group] groups
					ON			    groups.Id = discussion.Group_Id
					WHERE           comment.ThreadId = @ThreadId 
                    AND             groups.Slug = @Slug";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = groupSlug,
                ThreadId = threadId,
                UserId = userId
            });

            var commentsData = await reader.ReadAsync<CommentData>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, GenerateCommentModelFromData(commentsData));
        }

        private IEnumerable<Comment> GenerateCommentModelFromData(IEnumerable<CommentData> commentData)
        {
            return commentData.Select
            (
                item => new Comment()
                {
                    Id = item.Id,
                    Content = item.Content,
                    RepliesCount = item.RepliesCount,
                    LikesCount = item.Likes,
                    FirstRegistered = new Models.Shared.Properties
                    {
                        AtUtc = item.CreatedAtUtc,
                        By = new UserNavProperty
                        {
                            Id = item.CreatedById,
                            Name = item.CreatedByName,
                            Slug = item.CreatedBySlug
                        }
                    },
                    CurrentUser = new UserCommentDetails
                    {
                        Created = item.CreatedByThisUser,
                        Liked = item.LikedByThisUser
                    }
                });
        }
    }
}
