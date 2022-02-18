using Dapper;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using System.Data;

namespace FutureNHS.Api.DataAccess.Repositories.Write
{
    public sealed class FolderCommand : IFolderCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<FolderCommand> _logger;

        public FolderCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<FolderCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateFolderAsync(Guid userId, Guid groupId, FolderDto folder, CancellationToken cancellationToken)
        {
            const string query =
                 @" INSERT INTO  [dbo].[Folder]
                                 ([Title]
                                 ,[Description]
                                 ,[CreatedBy]
                                 ,[CreatedAtUTC]
                                 ,[ModifiedBy]
                                 ,[ModifiedAtUTC]
                                 ,[ParentFolder]
                                 ,[Group_Id]
                                 ,[IsDeleted])
                    VALUES
                                 (@Title
                                 ,@Description
                                 ,@CreatedBy
                                 ,@CreatedAtUTC
                                 ,@ModifiedBy
                                 ,@ModifiedAtUTC
                                 ,@ParentFolder
                                 ,@Group_Id
                                 ,@IsDeleted)";

            var queryDefinition = new CommandDefinition(query, new
            {
                Title = folder.Title,
                Description = folder.Description,
                CreatedBy = folder.CreatedBy,
                CreatedAtUTC = folder.CreatedAtUTC,
                ModifiedBy = folder.ModifiedBy,
                ModifiedAtUTC = folder.ModifiedAtUTC,
                ParentFolder = folder.ParentFolder,
                Group_Id = folder.GroupId,
                IsDeleted = folder.IsDeleted,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: CreateFolderAsync User:{0} request to add folder to group:{1} was not successful", userId, groupId);
                throw new DBConcurrencyException("Error: User request to add folder was not successful");
            }
        }
    }
}
