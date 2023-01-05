using System.Data;
using System.Diagnostics;
using System.Text;
using Dapper;
using FileServer.DataAccess.Interfaces;
using FileServer.Models;
using FileServer.PlatformHelpers.Interfaces;
using Microsoft.Data.SqlClient;

namespace FileServer.DataAccess.Sql.Read
{
    public class FileMetaDataProvider : IFileMetaDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<FileMetaDataProvider> _logger;
        
        public FileMetaDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<FileMetaDataProvider> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        
        public async Task<UserFileMetadata> GetFileMetaDataForUserAsync(Guid fileId, Guid userId, CancellationToken cancellationToken)
        {   
            cancellationToken.ThrowIfCancellationRequested();

            if (fileId == Guid.Empty) throw new ArgumentNullException(nameof(fileId));
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            
            const string query =
                $@"SELECT       [{nameof(UserFileMetadata.FileId)}]                = files.[Id],
                                [{nameof(UserFileMetadata.Title)}]                 = files.[Title],
                                [{nameof(UserFileMetadata.Description)}]           = files.[Description],
                                [{nameof(UserFileMetadata.GroupName)}]             = groups.[Name],
                                [{nameof(UserFileMetadata.Name)}]                  = files.[FileName],           
                                [{nameof(UserFileMetadata.SizeInBytes)}]           = files.[FileSizeBytes], 
                                [{nameof(UserFileMetadata.Extension)}]             = files.[FileExtension],
                                [{nameof(UserFileMetadata.BlobName)}]              = files.[BlobName],     
                                [{nameof(UserFileMetadata.ContentHash)}]           = files.[BlobHash], 
                                [{nameof(UserFileMetadata.LastWriteTimeUtc)}]      = CONVERT(DATETIMEOFFSET, ISNULL(files.[ModifiedAtUtc], files.[CreatedAtUtc])),
                                [{nameof(UserFileMetadata.OwnerUserName)}]         = owner.[UserName]

                    FROM      dbo.[File]           files
                    JOIN      dbo.[MembershipUser] owner ON owner.[Id] = files.[CreatedBy]
                    JOIN      dbo.[Folder]         folder ON folder.[Id] = files.[ParentFolder] AND folder.[IsDeleted] = 0
                    JOIN      dbo.[Group]          groups ON groups.[Id] = folder.[Group_Id]  AND groups.[IsDeleted] = 0
                    JOIN      dbo.[FileStatus]     fileStatus ON files.[FileStatus] = fileStatus.[Id]
                    LEFT JOIN dbo.[MembershipUser] membershipUser ON membershipUser.[Id] = @UserId AND membershipUser.[IsApproved] = 1
                    LEFT JOIN dbo.[GroupUser]      groupUser ON groupUser.[Group_Id] = groups.[Id] AND groupUser.[MembershipUser_Id] = membershipUser.[Id] AND groupUser.[Approved] = 1
         
                    WHERE  files.[Id]               = @FileId
                    AND    fileStatus.[Name]        = 'Verified'
                    AND    files.[IsDeleted]        = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                FileId = fileId,
                UserId = userId

            }, cancellationToken: cancellationToken);

            var metaData = await dbConnection.QueryFirstOrDefaultAsync<UserFileMetadata>(commandDefinition);

            if (metaData is null)
            {
                _logger?.LogError($"Failed to get the file ({fileId}) for user ({userId})");
                throw new FileNotFoundException("Could not get the meta data for the file");
            }

            return metaData;
        }
    
    
    public async Task UpdateFileMetaDataForUserAsync(Guid fileId, Guid userId,AzureBlobMetadata blobMetadata, DateTime modifiedAtUtc, CancellationToken cancellationToken)
        {   
            cancellationToken.ThrowIfCancellationRequested();

            if (fileId == Guid.Empty) throw new ArgumentNullException(nameof(fileId));
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            
            // get metadata for superseded file version from file.sql
            const string currentFileQuery =
                $@"SELECT       [{nameof(FileHistoryMetadata.FileId)}]                = files.[Id],
                                [{nameof(FileHistoryMetadata.Title)}]                 = files.[Title],
                                [{nameof(FileHistoryMetadata.Description)}]           = files.[Description],
                                [{nameof(FileHistoryMetadata.FileName)}]              = files.[FileName],           
                                [{nameof(FileHistoryMetadata.FileSizeBytes)}]         = files.[FileSizeBytes], 
                                [{nameof(FileHistoryMetadata.FileExtension)}]         = files.[FileExtension],
                                [{nameof(FileHistoryMetadata.BlobName)}]              = files.[BlobName],  
                                [{nameof(FileHistoryMetadata.ModifiedBy)}]            = files.[ModifiedBy],
                                [{nameof(FileHistoryMetadata.ModifiedAtUtc)}]         = files.[ModifiedAtUtc],
                                [{nameof(FileHistoryMetadata.FileStatus)}]            = files.[FileStatus],
                                [{nameof(FileHistoryMetadata.BlobHash)}]              = files.[BlobHash], 
                                [{nameof(FileHistoryMetadata.VersionId)}]             = files.[VersionId],
                                [{nameof(FileHistoryMetadata.IsDeleted)}]             = files.[IsDeleted],
                                [{nameof(FileHistoryMetadata.RowVersion)}]            = files.[RowVersion]


                    FROM      dbo.[File]           files
         
                    WHERE  files.[Id]               = @FileId
                    AND    files.[IsDeleted]        = 0;";
            
            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            await using var connection = new SqlConnection(dbConnection.ConnectionString);
            
            await connection.OpenAsync(cancellationToken);

           var currentFileCommandDefinition = new CommandDefinition(currentFileQuery, new
            {
                FileId = fileId,

            }, cancellationToken: cancellationToken);

            var currentFileMetaData = await connection.QueryFirstOrDefaultAsync<FileHistoryMetadata>(currentFileCommandDefinition);

            if (currentFileMetaData is null)
            {
                _logger?.LogError($"Failed to get the file ({fileId}) for user ({userId}) from table File");
                throw new FileNotFoundException("Could not get the meta data for the file");
            }
            var transaction = await connection.BeginTransactionAsync(cancellationToken);
            
            // Add metadata for the recently superseded file to FileHistory.sql
            const string fileHistoryQuery =
                $@" INSERT INTO [dbo].[FileHistory]
                                ([FileID],
                                [Title], 
                                [Description], 
                                [FileName],   
                                [FileSizeBytes],
                                [FileExtension],
                                [BlobName],
                                [ModifiedBy],
                                [ModifiedAtUtc],
                                [FileStatus],
                                [BlobHash],
                                [VersionId],
                                [IsDeleted])
            
                VALUES          (@FileId,
                                @Title,
                                @Description,
                                @FileName, 
                                @FileSizeBytes,
                                @FileExtension,
                                @BlobName,
                                @ModifiedBy,
                                @ModifiedAtUtc,
                                @FileStatus,
                                @BlobHash,
                                @VersionId,
                                @IsDeleted)";

            
                    
            var fileHistoryUpdated = await connection.ExecuteAsync(fileHistoryQuery, new
            {
                FileId =  currentFileMetaData.FileId,
                Title =  currentFileMetaData.Title,
                Description = currentFileMetaData.Description,
                FileName = currentFileMetaData.FileName,
                FileSizeBytes = currentFileMetaData.FileSizeBytes,
                FileExtension =  currentFileMetaData.FileExtension,
                BlobName =  currentFileMetaData.BlobName,
                ModifiedBy =  currentFileMetaData.ModifiedBy,
                ModifiedAtUtc =  currentFileMetaData.ModifiedAtUtc,
                FileStatus =  currentFileMetaData.FileStatus,
                BlobHash =  currentFileMetaData.BlobHash,
                VersionId =  currentFileMetaData.VersionId,
                IsDeleted =  currentFileMetaData.IsDeleted
                
            },transaction: transaction);

            if (fileHistoryUpdated != 1)
            {
                throw new DBConcurrencyException("Failed to update the fileHistory table");
            }
            ;
        
            
            
            const string query =
                $@" UPDATE      [dbo].[File]
                    SET         [ModifiedBy] = @UserId,
                                [ModifiedAtUtc] = @UpdatedDateUTC,
                                [BlobHash] = @ContentHash,
                                [VersionID] = @VersionId
                    WHERE       [Id] = @FileId ";

            var commandDefinition = new CommandDefinition(query, new
            {
                FileId = fileId,
                UserId = userId,
                ContentHash = blobMetadata.ContentHash,
                VersionId = blobMetadata.VersionId,
                UpdatedDateUTC = modifiedAtUtc

            },transaction: transaction, cancellationToken: cancellationToken);

            var updated = await dbConnection.ExecuteAsync(commandDefinition);

            if (updated != 1)
            {
                throw new DBConcurrencyException("Failed to update the file");
            }
            await transaction.CommitAsync(cancellationToken);

        }
    }
}