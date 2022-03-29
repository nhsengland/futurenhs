using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Comment;
using FutureNHS.Api.Exceptions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
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
                                [{nameof(CommentData.Id)}]                  = comment.Entity_Id,
                                [{nameof(CommentData.Content)}]             = comment.Content,          
                                [{nameof(CommentData.CreatedAtUtc)}]        = FORMAT(comment.CreatedAtUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(CommentData.CreatedById)}]         = comment.CreatedBy,
                                [{nameof(CommentData.RowVersion)}]          = comment.RowVersion

                    FROM            Comment comment	
					WHERE           comment.Entity_Id = @commentId
                    AND             comment.IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commentData = await dbConnection.QuerySingleAsync<CommentData>(query, new
            {
                commentId
            });

            if (commentData is null)
            {
                _logger.LogError($"Not Found: Comment:{0} not found", commentId);
                throw new NotFoundException("Not Found: Comment not found");
            }

            return commentData;
        }

        public async Task CreateCommentAsync(CommentDto comment, CancellationToken cancellationToken)
        {
            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            await using var connection = new SqlConnection(dbConnection.ConnectionString);

            const string insertEntity =
                @"  
                    INSERT INTO  [dbo].[Entity]
                                 ([Id])
                    VALUES
                                 (@EntityId)
                   ";

            const string insertComment =
                @"  
	                INSERT INTO  [dbo].[Comment]
                                 ([Entity_Id]
                                 ,[Content]
                                 ,[CreatedBy]
                                 ,[CreatedAtUTC]
                                 ,[ModifiedBy]
                                 ,[ModifiedAtUTC]
                                 ,[FlaggedAsSpam]
                                 ,[InReplyTo]
                                 ,[ThreadId]
                                 ,[IsDeleted]
                                 ,[Parent_EntityId])
                    VALUES
                                 (@EntityId
                                 ,@Content
                                 ,@CreatedBy
                                 ,@CreatedAtUTC
                                 ,@ModifiedBy
                                 ,@ModifiedAtUTC
                                 ,@FlaggedAsSpam
                                 ,@InReplyTo                                 
                                 ,@ThreadId
                                 ,@IsDeleted
                                 ,@ParentEntityId)";

            await connection.OpenAsync(cancellationToken);

            await using var transaction = connection.BeginTransaction();

            var insertEntityResult = await connection.ExecuteAsync(insertEntity, new
            {
                EntityId = comment.EntityId,
            }, transaction: transaction);

            var insertCommentResult = await connection.ExecuteAsync(insertComment, new
            {
                EntityId = comment.EntityId,
                Content = comment.Content,
                CreatedBy = comment.CreatedBy,
                CreatedAtUTC = comment.CreatedAtUTC,
                ModifiedBy = comment.ModifiedBy,
                ModifiedAtUTC = comment.ModifiedAtUTC,
                FlaggedAsSpam = comment.FlaggedAsSpam,
                InReplyTo = comment.InReplyTo,
                ThreadId = comment.ThreadId,
                IsDeleted = comment.IsDeleted,
                ParentEntityId = comment.DiscussionId
            }, transaction: transaction);

            if (insertCommentResult != 1)
            {
                _logger.LogError("Error: User request to create was not successful.", insertComment);
                throw new DataException("Error: User request to create was not successful.");

            }

            await transaction.CommitAsync(cancellationToken);
        }

        public async Task UpdateCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken)
        {
            const string query =

                @"  
	                UPDATE        [dbo].[Comment]
                    SET 
                                  [Content] = @Content
                                 ,[ModifiedBy] = @ModifiedBy
                                 ,[ModifiedAtUTC] = @ModifiedAtUtc
                    
                    WHERE 
                                 [Entity_Id] = @CommentId
                    AND          [RowVersion] = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                CommentId = comment.EntityId,
                Content = comment.Content,
                ModifiedBy = comment.ModifiedBy,
                ModifiedAtUTC = comment.ModifiedAtUTC,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to edit a comment was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to edit a comment was not successful");
            }
        }

        public async Task DeleteCommentAsync(CommentDto comment, byte[] rowVersion, CancellationToken cancellationToken = default)
        {
            const string query =
                @"  
	                UPDATE        [dbo].[Comment]
                    SET                                   
                                  [ModifiedBy] = @ModifiedBy
                                 ,[ModifiedAtUTC] = @ModifiedAtUtc
                                 ,[IsDeleted] = 1
                    
                    WHERE 
                                 [Entity_Id] = @CommentId
                    AND          [RowVersion] = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                CommentId = comment.EntityId,
                ModifiedBy = comment.ModifiedBy,
                ModifiedAtUTC = comment.ModifiedAtUTC,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to delete a comment was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to delete a comment was not successful");
            }
        }

        public async Task<Guid?> GetThreadIdForComment(Guid commentId, CancellationToken cancellationToken)
        {
            // Work up the chain using the InReplyTo column to find the original comment to find the threadId
            const string query =
                @" WITH            Comments AS 
                    (
                    SELECT      
                                    Entity_Id,
                                    InReplyTo

                    FROM            Comment
                    WHERE           Entity_Id = @Entity_Id
                    UNION ALL
                    SELECT
                                    comment.Entity_Id AS PK,
                                    comment.InReplyTo AS ParentFK

                    FROM            Comment comment
                    INNER JOIN      Comments comments 
                    ON              Comments.InReplyTo = comment.Entity_Id)

                    SELECT
                                    Entity_Id  
                    FROM            Comments 
                    WHERE           InReplyTo 
                    IS              NULL;";

            var queryDefinition = new CommandDefinition(query, new
            {
                Entity_Id = commentId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var result = await dbConnection.QueryFirstOrDefaultAsync<Guid?>(queryDefinition);

            return result;
        }
    }
}
