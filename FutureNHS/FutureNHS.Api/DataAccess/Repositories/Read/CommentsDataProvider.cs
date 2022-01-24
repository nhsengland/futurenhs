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
                                [{nameof(CommentData.Id)}]                  = post.Id,
                                [{nameof(CommentData.Content)}]             = post.PostContent, 
	                            [{nameof(CommentData.CreatedByThisUser)}]	= ( SELECT      CASE 
                                                                                WHEN        post.MembershipUser_Id = @UserId 
                                                                                THEN        CAST(1 as bit) 
                                                                                ELSE        CAST(0 as bit) 
                                                                                END
                                                                              ),               
                                [{nameof(CommentData.CreatedAtUtc)}]        = FORMAT(post.DateCreated,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(CommentData.CreatedById)}]         = post.MembershipUser_Id,
                                [{nameof(CommentData.CreatedByName)}]       = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(CommentData.CreatedBySlug)}]       = createUser.Slug,
								[{nameof(CommentData.RepliesCount)}]        = ( SELECT      COUNT(*) 
                                                                                FROM        Post replies 
                                                                                WHERE       replies.ThreadId = post.Id
                                                                              ),
								[{nameof(CommentData.Likes)}]				= ( SELECT      COUNT(*) 
                                                                                FROM        Vote 
                                                                                WHERE       Post_Id = post.Id
                                                                              ),
								[{nameof(CommentData.LikedByThisUser)}]		= ( SELECT      CASE 
                                                                                WHEN        Id IS NULL 
                                                                                THEN        CAST(0 as bit) 
                                                                                ELSE        CAST(1 as bit) 
                                                                                END 
                                                                                FROM        Vote   
                                                                                WHERE       Vote.Post_Id = post.Id 
                                                                                AND         Vote.MembershipUser_Id = @UserId
                                                                              )

                    FROM            Post post
					Join		    Topic topic
					ON			    topic.Id = post.Topic_Id
					JOIN		    [Group] groups
					ON			    groups.Id = Topic.Group_Id
                    LEFT JOIN       MembershipUser createUser 
                    ON              CreateUser.Id = post.MembershipUser_Id		
					WHERE           post.Topic_Id = @TopicId 
                    AND             post.ThreadId IS NULL
                    AND             groups.Slug = @Slug
                    ORDER BY        post.DateCreated

                    OFFSET          @Offset ROWS
                    FETCH NEXT      @Limit ROWS ONLY;

                    SELECT          COUNT(*) 

                    FROM            Post post		
					Join		    Topic topic
					ON			    topic.Id = post.Topic_Id
					JOIN		    [Group] groups
					ON			    groups.Id = Topic.Group_Id
					WHERE           post.Topic_Id = @TopicId 
                    AND             post.IsTopicStarter = 0
                    AND             post.ThreadId IS NULL
                    AND             groups.Slug = @Slug";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = groupSlug,
                TopicId = topicId,
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
                                [{nameof(CommentData.Id)}]                  = post.Id,
                                [{nameof(CommentData.Content)}]             = post.PostContent, 
	                            [{nameof(CommentData.CreatedByThisUser)}]	= ( SELECT      CASE 
                                                                                WHEN        post.MembershipUser_Id = @UserId
                                                                                THEN        CAST(1 as bit) 
                                                                                ELSE        CAST(0 as bit) 
                                                                                END
                                                                              ),               
                                [{nameof(CommentData.CreatedAtUtc)}]        = FORMAT(post.DateCreated,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(CommentData.CreatedById)}]         = post.MembershipUser_Id,
                                [{nameof(CommentData.CreatedByName)}]       = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(CommentData.CreatedBySlug)}]       = createUser.Slug,
								[{nameof(CommentData.Likes)}]				= ( SELECT      COUNT(*) 
                                                                                FROM        Vote 
                                                                                WHERE       Post_Id = post.Id
                                                                              ),
								[{nameof(CommentData.LikedByThisUser)}]		= ( SELECT      CASE 
                                                                                WHEN        Id IS NULL 
                                                                                THEN        CAST(0 as bit) 
                                                                                ELSE        CAST(1 as bit) 
                                                                                END 
                                                                                FROM        Vote   
                                                                                WHERE       Vote.Post_Id = post.Id 
                                                                                AND         Vote.MembershipUser_Id = @UserId
                                                                              )

                    FROM            Post post
					Join		    Topic topic
					ON			    topic.Id = post.Topic_Id
					JOIN		    [Group] groups
					ON			    groups.Id = Topic.Group_Id
                    LEFT JOIN       MembershipUser createUser 
                    ON              CreateUser.Id = post.MembershipUser_Id		
					WHERE           post.ThreadId = @ThreadId 
                    AND             groups.Slug = @Slug
                    ORDER BY        post.DateCreated

                    OFFSET          @Offset ROWS
                    FETCH NEXT      @Limit ROWS ONLY;

                    SELECT          COUNT(*) 

                    FROM            Post post		
					Join		    Topic topic
					ON			    topic.Id = post.Topic_Id
					JOIN		    [Group] groups
					ON			    groups.Id = Topic.Group_Id
					WHERE           post.ThreadId = @ThreadId 
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
