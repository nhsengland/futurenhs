using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Exceptions;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
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

        public async Task<FolderDto> GetFolderAsync(Guid folderId, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT
                                [{nameof(FolderDto.Id)}]                = folder.Id,
                                [{nameof(FolderDto.Title)}]             = folder.Title,
                                [{nameof(FolderDto.Description)}]       = folder.Description,
                                [{nameof(FolderDto.CreatedBy)}]         = folder.CreatedBy,
                                [{nameof(FolderDto.CreatedAtUTC)}]      = FORMAT(folder.CreatedAtUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(FolderDto.ModifiedBy)}]        = folder.ModifiedBy,
                                [{nameof(FolderDto.ModifiedAtUTC)}]     = FORMAT(folder.ModifiedAtUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(FolderDto.ParentFolder)}]      = folder.ParentFolder,
                                [{nameof(FolderDto.FileCount)}]         = folder.FileCount,
                                [{nameof(FolderDto.GroupId)}]           = folder.Group_Id,
                                [{nameof(FolderDto.IsDeleted)}]         = folder.IsDeleted,
                                [{nameof(FolderDto.RowVersion)}]        = folder.RowVersion

                    FROM        [dbo].[Folder] folder	
					WHERE       folder.Id = @folderId;";

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var commentData = await dbConnection.QuerySingleAsync<FolderDto>(query, new
            {
                folderId
            });

            if (commentData is null)
            {
                _logger.LogError($"Not Found: Folder:{0} not found", folderId);
                throw new NotFoundException("Not Found: Folder not found");
            }

            return commentData;
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

        public async Task UpdateFolderAsync(Guid userId, FolderDto folder, byte[] rowVersion, CancellationToken cancellationToken)
        {
            const string query =
                 @" Update        [dbo].[Folder]
                    SET
                                  [Title] = @Title
                                 ,[Description] = @Description
                                 ,[ModifiedBy] = @ModifiedBy
                                 ,[ModifiedAtUTC] = @ModifiedAtUTC
                    WHERE 
                                 [Id] = @FolderId
                    AND          [RowVersion] = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                FolderId = folder.Id,
                Title = folder.Title,
                Description = folder.Description,
                ModifiedBy = folder.ModifiedBy,
                ModifiedAtUTC = folder.ModifiedAtUTC,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: UpdateFolderAsync User:{0} request to edit folder:{1} was not successful", userId, folder.Id);
                throw new DBConcurrencyException("Error: User request to edit folder was not successful");
            }
        }

        public async Task<bool> IsFolderUniqueAsync(string folderTitle, Guid? parentFolderId, Guid groupId, CancellationToken cancellationToken)
        {
            const string query =
                 @" SELECT        CAST(COUNT(1) AS BIT)
                    FROM
                                  Folder f
                    WHERE
                                  f.Title = @Title
                    AND           f.IsDeleted = 0
                    AND           (
                                      f.ParentFolder = @ParentGroupId
                                      OR
                                      (@ParentGroupId IS NULL AND f.ParentFolder IS NULL)
                                  ) 
                    AND           f.Group_Id = @GroupId";

            var queryDefinition = new CommandDefinition(query, new
            {
                Title = folderTitle,
                GroupId = groupId,
                ParentGroupId = parentFolderId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            return !await dbConnection.QuerySingleOrDefaultAsync<bool>(queryDefinition);
        }
    }
}
