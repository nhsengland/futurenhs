using System.Data;
using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public sealed class FileCommand : IFileCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<FileCommand> _logger;

        public FileCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<FileCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateFileAsync(FileDto file, CancellationToken cancellationToken)
        {
            const string query =
                 @" INSERT INTO  [dbo].[File]
                                 ([Title]
                                 ,[Description]
                                 ,[FileName]
                                 ,[FileSizeBytes]
                                 ,[FileExtension]
                                 ,[BlobName]
                                 ,[CreatedBy]
                                 ,[CreatedAtUTC]
                                 ,[ModifiedBy]
                                 ,[ModifiedAtUTC]
                                 ,[ParentFolder]
                                 ,[FileStatus]
                                 ,[BlobHash]
                                 ,[IsDeleted])
                    VALUES
                                 (@Title
                                 ,@Description
                                 ,@FileName
                                 ,@FileSizeBytes
                                 ,@FileExtension
                                 ,@BlobName
                                 ,@CreatedBy
                                 ,@CreatedAtUTC
                                 ,@ModifiedBy
                                 ,@ModifiedAtUTC
                                 ,@ParentFolder
                                 ,@FileStatus
                                 ,@BlobHash
                                 ,@IsDeleted)";

            var queryDefinition = new CommandDefinition(query, new
            {
                Title = file.Title,
                Description = file.Description,
                FileName = file.FileName,
                FileSizeBytes = file.FileSizeBytes,
                FileExtension = file.FileExtension,
                BlobName = file.BlobName,
                CreatedBy = file.CreatedBy,
                CreatedAtUTC = file.CreatedAtUTC,
                ModifiedBy = file.ModifiedBy,
                ModifiedAtUTC = file.ModifiedAtUTC,
                ParentFolder = file.ParentFolder,
                FileStatus = file.FileStatus,
                BlobHash = file.BlobHash,
                IsDeleted = file.IsDeleted,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: CreateFileAsync User:{0} request to add file to folder:{1} was not successful", file.CreatedBy, file.ParentFolder);
                throw new DBConcurrencyException("Error: User request to add file was not successful");
            }
        }

        public async Task<Guid> GetFileStatus(string fileStatus, CancellationToken cancellationToken)
        {
            const string query =
                @"SELECT        ID
                  FROM          [FileStatus]
                  WHERE         Name = @FileStatus";

            var queryDefinition = new CommandDefinition(query, new
            {
                FileStatus = fileStatus
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var result = await dbConnection.QueryFirstAsync<Guid>(queryDefinition);

            return result;
        }
    }
}
