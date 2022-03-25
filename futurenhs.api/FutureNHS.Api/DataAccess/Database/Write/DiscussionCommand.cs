using System.Data;
using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;

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

        public async Task CreateDiscussionAsync(DiscussionDto discussion,  CancellationToken cancellationToken = default)
        {
            const string query =
                 @" INSERT INTO     [dbo].[Discussion]
                                    ([Title]
                                    ,[CreatedAtUtc]
                                    ,[CreatedBy]
                                    ,[IsSticky]
                                    ,[IsLocked]
                                    ,[Group_Id]
                                    ,[Poll_Id]
                                    ,[Category_Id]
                                    ,[Content])
                    VALUES
                                    (@Title
                                    ,@CreatedAt
                                    ,@CreatedBy
                                    ,@IsSticky
                                    ,@IsLocked
                                    ,@Group
                                    ,NULL
                                    ,NULL
                                    ,@Content)";

            var queryDefinition = new CommandDefinition(query, new
            {
                Title = discussion.Title,
                CreatedAt = discussion.CreatedAtUTC,
                CreatedBy = discussion.CreatedBy,
                IsSticky = discussion.IsSticky,
                IsLocked = discussion.IsLocked,
                Group = discussion.GroupId,
                Content = discussion.Content

            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: Discussion was not added", queryDefinition);
                throw new DataException("Error: Discussion was not added");
            }
        }
    }
}
