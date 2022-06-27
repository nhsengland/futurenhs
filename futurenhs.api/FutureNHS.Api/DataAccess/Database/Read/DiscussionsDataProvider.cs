using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.DataAccess.Models.Discussions;
using FutureNHS.Api.DataAccess.Models.User;
using Microsoft.Extensions.Options;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class DiscussionDataProvider : IDiscussionDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<DiscussionDataProvider> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public DiscussionDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<DiscussionDataProvider> logger, IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _options = options;
        }

        public async Task<uint> GetDiscussionCountForGroupAsync(string groupSlug, CancellationToken cancellationToken)
        {
            const string query =
                $@"
                    SELECT      COUNT(*) 

                    FROM        Discussion discussion
					JOIN        [Group] groups
                    ON          groups.Id = discussion.Group_Id
					WHERE       groups.Slug = @Slug
                    AND         discussion.IsDeleted = 0
                    AND         groups.IsDeleted = 0;
                ";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                Slug = groupSlug
            }, cancellationToken: cancellationToken);

            return await dbConnection.QuerySingleAsync<uint>(commandDefinition);
        }

        public async Task<IEnumerable<Discussion>?> GetDiscussionsForGroupAsync(Guid? userId, string groupSlug, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @$"SELECT
                                [{nameof(DiscussionData.Id)}]                   = discussion.Entity_Id,
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
                                [{nameof(DiscussionData.LastComment)}]			= latestComment.Entity_Id,
                                [{nameof(DiscussionData.LastCommentAtUtc)}]     = FORMAT(latestComment.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.LastCommenterId)}]      = latestComment.CreatedBy,
                                [{nameof(DiscussionData.LastCommenterName)}]    = lastCommentUser.FirstName + ' ' + lastCommentUser.Surname,
                                [{nameof(DiscussionData.LastCommenterSlug)}]    = lastCommentUser.Slug,
                                [{nameof(DiscussionData.IsSticky)}]				= discussion.IsSticky,
                                [{nameof(DiscussionData.Views)}]				= discussion.Views,
                                [{nameof(DiscussionData.TotalComments)}]		= (SELECT COUNT(*) FROM Comment WHERE Parent_EntityId = discussion.Entity_Id ),
                                [{nameof(ImageData.Id)}]		                = [image].Id,
                                [{nameof(ImageData.Height)}]	                = [image].Height,
                                [{nameof(ImageData.Width)}]		                = [image].Width,
                                [{nameof(ImageData.FileName)}]	                = [image].FileName,
                                [{nameof(ImageData.MediaType)}]	                = [image].MediaType

                    FROM        Discussion discussion
					JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id

                    LEFT JOIN   MembershipUser createdByUser 
                    ON          createdByUser.Id = discussion.CreatedBy

					LEFT JOIN   Image [image]
                    ON          [image].Id = createdByUser.ImageId   

					LEFT JOIN   
                                (
					            SELECT TOP 1 *
					            FROM Comment 
                                WHERE ThreadId IS NULL
					            ORDER BY CreatedAtUTC
					            ) 
                                latestComment ON latestComment.Parent_EntityId = discussion.Entity_Id

                    LEFT JOIN   MembershipUser lastCommentUser 
                    ON          lastCommentUser.Id = latestComment.CreatedBy

                    WHERE       groups.Slug = @Slug
                    AND         groups.IsDeleted = 0    
                    AND         discussion.IsDeleted = 0
                    ORDER BY    discussion.IsSticky DESC, discussion.CreatedAtUTC DESC

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);


            var results = await dbConnection.QueryAsync<DiscussionData, Image, DiscussionData>(query,
                (discussion, image) =>
                {
                    if (image is not null)
                    {
                        return discussion with { Image = new ImageData(image, _options) };
                    }

                    return @discussion;
                }, new
                {
                    Offset = Convert.ToInt32(offset),
                    Limit = Convert.ToInt32(limit),
                    Slug = groupSlug,
                    UserId = userId
                }, splitOn: "id");

            return GenerateDiscussionModelFromData(results);
        }

        public async Task<Discussion?> GetDiscussionAsync(Guid? userId, string groupSlug, Guid id, CancellationToken cancellationToken)
        {
            const string query =
                @$" SELECT
                                [{nameof(DiscussionData.Id)}]                   = discussion.Entity_Id,
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
                                [{nameof(DiscussionData.LastComment)}]			= latestComment.Entity_Id,
                                [{nameof(DiscussionData.LastCommentAtUtc)}]     = FORMAT(latestComment.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionData.LastCommenterId)}]      = latestComment.CreatedBy,
                                [{nameof(DiscussionData.LastCommenterName)}]    = lastCommentUser.FirstName + ' ' + lastCommentUser.Surname,
                                [{nameof(DiscussionData.LastCommenterSlug)}]    = lastCommentUser.Slug,
                                [{nameof(DiscussionData.IsSticky)}]				= discussion.IsSticky,
                                [{nameof(DiscussionData.Views)}]				= discussion.Views,
                                [{nameof(DiscussionData.TotalComments)}]		= (SELECT COUNT(*) FROM Comment WHERE Parent_EntityId = discussion.Entity_Id),
                                [{nameof(ImageData.Id)}]		                = [image].Id,
                                [{nameof(ImageData.Height)}]	                = [image].Height,
                                [{nameof(ImageData.Width)}]		                = [image].Width,
                                [{nameof(ImageData.FileName)}]	                = [image].FileName,
                                [{nameof(ImageData.MediaType)}]	                = [image].MediaType

                    FROM        Discussion discussion
                    JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id

                    LEFT JOIN   MembershipUser createdByUser 
                    ON          createdByUser.Id = discussion.CreatedBy

                    LEFT JOIN   Image [image]
                    ON          [image].Id = createdByUser.ImageId   
          
					LEFT JOIN   
                                (
					            SELECT TOP 1 *
					            FROM Comment 
                                WHERE ThreadId IS NULL
					            ORDER BY CreatedAtUTC
					            ) 
                                latestComment ON latestComment.Parent_EntityId = discussion.Entity_Id

                    LEFT JOIN   MembershipUser lastCommentUser 
                    ON          lastCommentUser.Id = latestComment.CreatedBy
                    
                    WHERE       discussion.Entity_Id = @Id 
                    AND         discussion.IsDeleted = 0
                    AND         groups.Slug = @Slug
                    AND         groups.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var results = await dbConnection.QueryAsync<DiscussionData, Image, DiscussionData>(query,
                            (discussion, image) =>
                            {
                                if (image is not null)
                                {
                                    return discussion with { Image = new ImageData(image, _options) };
                                }

                                return @discussion;
                            }, new
                            {
                                Slug = groupSlug,
                                Id = id,
                                UserId = userId
                            });

            return GenerateDiscussionModelFromData(results).FirstOrDefault();
        }        

        public async Task<DiscussionCreatorDetails> GetDiscussionCreatorDetailsAsync(Guid discussionId, CancellationToken cancellationToken)
        {
            const string query =
                @$" SELECT
                                [{nameof(DiscussionCreatorDetails.DiscussionId)}]         = discussion.Entity_Id,
                                [{nameof(DiscussionCreatorDetails.GroupSlug)}]            = groups.Slug,
                                [{nameof(DiscussionCreatorDetails.CreatedAtUtc)}]         = FORMAT(discussion.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(DiscussionCreatorDetails.CreatedById)}]          = discussion.CreatedBy,
                                [{nameof(DiscussionCreatorDetails.CreatedByName)}]        = createdByUser.FirstName + ' ' + createdByUser.Surname,
                                [{nameof(DiscussionCreatorDetails.CreatedByEmail)}]       = createdByUser.Email
                    
                    FROM        Discussion discussion
                    JOIN        [Group] groups 
                    ON          groups.Id = discussion.Group_Id

                    LEFT JOIN   MembershipUser createdByUser 
                    ON          createdByUser.Id = discussion.CreatedBy
                    
                    WHERE       discussion.Entity_Id = @Id 
                    AND         discussion.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                Id = discussionId
            }, cancellationToken: cancellationToken);

            var result = await dbConnection.QuerySingleOrDefaultAsync<DiscussionCreatorDetails>(commandDefinition);

            if (result is null)
            {
                _logger.LogError("Error: User request to get a discussion was not successful", commandDefinition);
                throw new DBConcurrencyException("Error: User request to get a discussion was not successful");
            }

            return result;
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
                            Slug = item.CreatedBySlug,
                            Image = item.Image

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
                                Slug = item.LastCommenterSlug,
                                Image = item.Image
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
