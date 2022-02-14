using Dapper;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using System.Data;

namespace FutureNHS.Api.DataAccess.Repositories.Write
{
    public sealed class CommentsCommand : ICommentsCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<CommentsCommand> _logger;

        public CommentsCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<CommentsCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateCommentAsync(PostDto post, CancellationToken cancellationToken = default)
        {
            const string query =
                 @" INSERT INTO  [dbo].[Post]
                                 ([Id]
                                 ,[PostContent]
                                 ,[DateCreated]
                                 ,[VoteCount]
                                 ,[DateEdited]
                                 ,[IsSolution]
                                 ,[IsTopicStarter]
                                 ,[FlaggedAsSpam]
                                 ,[Pending]
                                 ,[InReplyTo]
                                 ,[ExtendedDataString]
                                 ,[Topic_Id]
                                 ,[MembershipUser_Id]
                                 ,[ThreadId])
                    VALUES
                                 (NEWID()
                                 ,@PostContent
                                 ,SYSDATETIME()
                                 ,0
                                 ,SYSDATETIME()
                                 ,0
                                 ,0
                                 ,0
                                 ,0
                                 ,@InReplyTo
                                 ,NULL
                                 ,@TopicId
                                 ,@MembershipUserId
                                 ,@InReplyTo)";

            var queryDefinition = new CommandDefinition(query, new
            {
                PostContent = post.PostContent,
                InReplyTo = post.InReplyTo,
                TopicId = post.TopicId,
                MembershipUserId = post.MembershipUserId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to add a comment was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to add a comment was not successful");
            }
        }
    }
}
