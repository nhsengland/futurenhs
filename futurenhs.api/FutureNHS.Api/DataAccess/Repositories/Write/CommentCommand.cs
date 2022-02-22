using Dapper;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Exceptions;
using System.Data;

namespace FutureNHS.Api.DataAccess.Repositories.Write
{
    public sealed class CommentCommand : ICommentCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<CommentCommand> _logger;

        public CommentCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<CommentCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CommentData> GetCommentAsync(Guid commentId, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT
                                [{nameof(CommentData.Id)}]                  = comment.Id,
                                [{nameof(CommentData.Content)}]             = comment.Content,          
                                [{nameof(CommentData.CreatedAtUtc)}]        = FORMAT(comment.CreatedAtUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(CommentData.CreatedById)}]         = comment.CreatedBy,
                                [{nameof(CommentData.RowVersion)}]          = comment.RowVersion

                    FROM            Comment comment	
					WHERE           comment.Id = @commentId;";

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                commentId
            });

            var commentData = await reader.ReadSingleOrDefaultAsync<CommentData>();
            if (commentData is null)
            {
                _logger.LogError($"Not Found: Comment:{0} not found", commentId);
                throw new NotFoundException("Not Found: Comment not found");
            }

            return commentData;
        }

        public async Task CreateCommentAsync(CommentDto comment, CancellationToken cancellationToken)
        {
            const string query =
                 @" INSERT INTO  [dbo].[Comment]
                                 ([Content]
                                 ,[CreatedBy]
                                 ,[CreatedAtUTC]
                                 ,[ModifiedBy]
                                 ,[ModifiedAtUTC]
                                 ,[FlaggedAsSpam]
                                 ,[InReplyTo]
                                 ,[Discussion_Id]
                                 ,[ThreadId]
                                 ,[IsDeleted])
                    VALUES
                                 (@Content
                                 ,@CreatedBy
                                 ,@CreatedAtUTC
                                 ,@ModifiedBy
                                 ,@ModifiedAtUTC
                                 ,@FlaggedAsSpam
                                 ,@InReplyTo
                                 ,@DiscussionId
                                 ,@ThreadId
                                 ,@IsDeleted)";

            var queryDefinition = new CommandDefinition(query, new
            {
                Content = comment.Content,
                CreatedBy = comment.CreatedBy,
                CreatedAtUTC = comment.CreatedAtUTC,
                ModifiedBy = comment.ModifiedBy,
                ModifiedAtUTC = comment.ModifiedAtUTC,
                FlaggedAsSpam = comment.FlaggedAsSpam,
                InReplyTo = comment.InReplyTo,
                DiscussionId = comment.DiscussionId,
                ThreadId = comment.ThreadId,
                IsDeleted = comment.IsDeleted,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to add a comment was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to add a comment was not successful");
            }
        }
        public async Task UpdateCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken)
        {
            const string query =
                 @" UPDATE        [dbo].[Comment]
                    SET 
                                  [Content] = @Content
                                 ,[ModifiedBy] = @ModifiedBy
                                 ,[ModifiedAtUTC] = @ModifiedAtUtc
                    
                    WHERE 
                                 [Id] = @CommentId
                    AND          [RowVersion] = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                CommentId = comment.Id,
                Content = comment.Content,
                ModifiedBy = comment.ModifiedBy,
                ModifiedAtUTC = comment.ModifiedAtUTC,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to add a comment was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to add a comment was not successful");
            }
        }

        public async Task<Guid?> GetThreadIdForComment(Guid commentId, CancellationToken cancellationToken)
        {
            // Work up the chain using the InReplyTo column to find the original comment to find the threadId
            const string query =
                @" WITH            Comments AS 
                    (
                    SELECT      
                                    Id,
                                    InReplyTo

                    FROM            Comment
                    WHERE           Id = @CommentId
                    UNION ALL
                    SELECT
                                    comment.Id AS PK,
                                    comment.InReplyTo AS ParentFK

                    FROM            Comment comment
                    INNER JOIN      Comments comments 
                    ON              Comments.InReplyTo = comment.Id
                    )

                    SELECT
                                    Id  
                    FROM            Comments 
                    WHERE           InReplyTo 
                    IS              NULL;";

            var queryDefinition = new CommandDefinition(query, new
            {
                CommentId = commentId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.QueryFirstOrDefaultAsync<Guid?>(queryDefinition);

            return result;
        }
    }
}
