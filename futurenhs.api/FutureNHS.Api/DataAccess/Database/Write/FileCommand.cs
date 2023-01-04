using System.Data;
using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.FileAndFolder;
using FutureNHS.Api.Exceptions;

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
                                 ,[VersionId]
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
                                 ,@VersionId
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
                VersionId = file.VersionId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: CreateFileAsync User:{0} request to add file to folder:{1} was not successful", file.CreatedBy, file.ParentFolder);
                throw new DBConcurrencyException("Error: User request to add file was not successful");
            }
        }

        public async Task<FileDto> GetFileAsync(Guid id, string status, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT
                                [{nameof(FileDto.Id)}]                  = [file].Id,
                                [{nameof(FileDto.Title)}]               = [file].Title,          
                                [{nameof(FileDto.Description)}]         = [file].Description,
                                [{nameof(FileDto.FileName)}]            = [file].FileName,
                                [{nameof(FileDto.FileSizeBytes)}]       = [file].FileSizeBytes,
                                [{nameof(FileDto.FileExtension)}]       = [file].FileExtension,
                                [{nameof(FileDto.BlobName)}]            = [file].BlobName,          
                                [{nameof(FileDto.CreatedBy)}]           = [file].CreatedBy,
                                [{nameof(FileDto.CreatedAtUTC)}]        = [file].CreatedAtUTC,
                                [{nameof(FileDto.ModifiedBy)}]          = [file].ModifiedBy,
                                [{nameof(FileDto.ModifiedAtUTC)}]       = [file].ModifiedAtUTC,
                                [{nameof(FileDto.ParentFolder)}]        = [file].ParentFolder,
                                [{nameof(FileDto.FileStatus)}]          = [file].FileStatus,
                                [{nameof(FileDto.BlobHash)}]            = [file].BlobHash,
                                [{nameof(FileDto.IsDeleted)}]           = [file].IsDeleted          
             

                    FROM            [File] [file]	
					JOIN			FileStatus status 
					ON				status.Id = [file].FileStatus
					WHERE           [file].Id = @Id
                    AND             [file].IsDeleted = 0
                    AND             status.Name = @Status";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Id = id,
                Status = status
            });

            var fileDto = await reader.ReadSingleOrDefaultAsync<FileDto>();
            if (fileDto is null)
            {
                _logger.LogError($"Not Found: File:{0} not found", id);
                throw new NotFoundException("Not Found: File not found");
            }

            return fileDto;
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
        public async Task<AuthUserData> GetFileAccess(Guid userId,Guid fileId, CancellationToken cancellationToken)
        {
            const string query =
                @$"
                    SELECT 
                                [{nameof(AuthUserData.Id)}]                 = membershipUser.Id,
                                [{nameof(AuthUserData.EmailAddress)}]       = membershipUser.Email,          
                                [{nameof(AuthUserData.FullName)}]           = TRIM(ISNULL(membershipUser.FirstName, '') + ' ' + ISNULL(membershipUser.Surname, '')),
                                [{nameof(AuthUserData.Initials)}]           = membershipUser.Initials, 
                                [{nameof(AuthUserData.AvatarUrl)}]          = [image].FileName, 
                                [{nameof(AuthUserData.FileId)}]             = files.Id, 
                                [{nameof(AuthUserData.GroupSlug)}]          = groups.Slug 

                    FROM        MembershipUser membershipUser
                    JOIN        [File] files 
                    on          files.Id = @FileId
                    JOIN        Folder folder 
                    on          folder.Id = files.ParentFolder
                    JOIN        [Group] groups 
                    on          groups.Id = folder.Group_Id
                    LEFT JOIN   Image [image]
                    ON          [image].Id = membershipUser.ImageId  
                    WHERE       membershipUser.Id = @UserId
                ";

            var queryDefinition = new CommandDefinition(query, new
            {
                UserId = userId,
                FileId = fileId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var result = await dbConnection.QueryFirstOrDefaultAsync<AuthUserData>(queryDefinition);
            return result;
        }
        
        public async Task<AuthUserData> GetFileVersionAccess(Guid userId,Guid fileId, CancellationToken cancellationToken)
        {
            const string query =
                @$"
                    SELECT 
                                [{nameof(AuthUserData.Id)}]                 = membershipUser.Id,
                                [{nameof(AuthUserData.EmailAddress)}]       = membershipUser.Email,          
                                [{nameof(AuthUserData.FullName)}]           = TRIM(ISNULL(membershipUser.FirstName, '') + ' ' + ISNULL(membershipUser.Surname, '')),
                                [{nameof(AuthUserData.Initials)}]           = membershipUser.Initials,
                                [{nameof(AuthUserData.AvatarUrl)}]          = [image].FileName, 
                                [{nameof(AuthUserData.FileId)}]             = fileHistory.Id, 
                                [{nameof(AuthUserData.GroupSlug)}]          = groups.Slug 

                    FROM MembershipUser membershipUser
                    JOIN [FileHistory] fileHistory on fileHistory.Id = @FileId
                    JOIN [File] files on files.Id = fileHistory.FileId
                    JOIN Folder folder on folder.Id = files.ParentFolder
                    JOIN [Group] groups on groups.Id = folder.Group_Id
                    LEFT JOIN   Image [image]
                    ON          [image].Id = membershipUser.ImageId  
                    WHERE membershipUser.Id = @UserId
                ";

            var queryDefinition = new CommandDefinition(query, new
            {
                UserId = userId,
                FileId = fileId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var result = await dbConnection.QueryFirstAsync<AuthUserData>(queryDefinition);

            return result;
        }
    }
}
