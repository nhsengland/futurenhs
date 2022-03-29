using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class DiscussionCommand : IDiscussionCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<DiscussionCommand> _logger;

        public DiscussionCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<DiscussionCommand> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;

        }

        public async Task CreateDiscussionAsync(DiscussionDto discussion, CancellationToken cancellationToken = default)
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

            const string insertDiscussion =
                @" INSERT INTO     [dbo].[Discussion]
                                    ([Entity_Id]
                                    ,[Title]
                                    ,[CreatedAtUtc]
                                    ,[CreatedBy]
                                    ,[IsSticky]
                                    ,[IsLocked]
                                    ,[Group_Id]
                                    ,[Poll_Id]
                                    ,[Category_Id]
                                    ,[Content])
                    VALUES
                                    (@Id
                                    ,@Title
                                    ,@CreatedAt
                                    ,@CreatedBy
                                    ,@IsSticky
                                    ,@IsLocked
                                    ,@Group
                                    ,NULL
                                    ,NULL
                                    ,@Content)";

            await connection.OpenAsync(cancellationToken);

            var transaction = await connection.BeginTransactionAsync(cancellationToken);

            var insertEntityResult = await connection.ExecuteAsync(insertEntity, new
            {
                EntityId = discussion.Id,
            }, transaction: transaction);

            var insertDiscussionResult = await connection.ExecuteAsync(insertDiscussion, new
            {
                Id = discussion.Id,
                Title = discussion.Title,
                CreatedAt = discussion.CreatedAtUTC,
                CreatedBy = discussion.CreatedBy,
                IsSticky = discussion.IsSticky,
                IsLocked = discussion.IsLocked,
                Group = discussion.GroupId,
                Content = discussion.Content
            }, transaction: transaction);

            if (insertEntityResult != 1)
            {
                _logger.LogError("Error: User request to create was not successful.", insertEntity);
                throw new DataException("Error: User request to create was not successful.");
            }

            if (insertDiscussionResult != 1)
            {
                _logger.LogError("Error: User request to create was not successful.", insertDiscussion);
                throw new DataException("Error: User request to create was not successful.");
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
