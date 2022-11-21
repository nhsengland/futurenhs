using System.Data;
using System.Diagnostics;
using System.Text;
using Dapper;
using FileServer.DataAccess.Interfaces;
using FileServer.Models;
using FileServer.PlatformHelpers.Interfaces;

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
    
    
    public async Task UpdateFileMetaDataForUserAsync(Guid fileId, Guid userId,byte[] contentHash, DateTime modifiedAtutc, CancellationToken cancellationToken)
        {   
            cancellationToken.ThrowIfCancellationRequested();

            if (fileId == Guid.Empty) throw new ArgumentNullException(nameof(fileId));
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            
            const string query =
                $@" UPDATE      [dbo].[File]
                    SET         [ModifiedBy] = @UserId,
                                [ModifiedAtUtc] = @UpdatedDateUTC,
                                [BlobHash] = @ContentHash
                    WHERE       [Id] = @FileId ";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                FileId = fileId,
                UserId = userId,
                ContentHash = contentHash,
                UpdatedDateUTC = modifiedAtutc

            }, cancellationToken: cancellationToken);

            var updated = await dbConnection.ExecuteAsync(commandDefinition);

            if (updated != 1)
            {
                throw new DBConcurrencyException("Failed to update the file");
            }

;
        }
    }
}