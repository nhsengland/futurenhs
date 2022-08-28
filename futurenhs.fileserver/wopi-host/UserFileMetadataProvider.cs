using FutureNHS.WOPIHost.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    public interface IUserFileMetadataProvider
    {

        /// <summary>
        /// Tasked with retrieving the extended metadata for a specific file version
        /// </summary>
        /// <param name="file">The details of the file and version for which the extended metadata is being requested</param>
        /// <param name="authenticatedUser">The user for whom the metadata is being sought</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The requested metadata in the success case</returns>
        Task<UserFileMetadata> GetForFileAsync(File file, AuthenticatedUser authenticatedUser, CancellationToken cancellationToken);
    }

    public sealed class UserFileMetadataProvider : IUserFileMetadataProvider
    {
        private readonly IAzureSqlClient _azureSqlClient;
        private readonly ILogger<UserFileMetadataProvider>? _logger;

        public UserFileMetadataProvider(IAzureSqlClient azureSqlClient, ILogger<UserFileMetadataProvider>? logger)
        {
            _logger = logger;

            _azureSqlClient = azureSqlClient ?? throw new ArgumentNullException(nameof(azureSqlClient));
        }

        async Task<UserFileMetadata?> IUserFileMetadataProvider.GetForFileAsync(File file, AuthenticatedUser authenticatedUser, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (file.IsEmpty) throw new ArgumentNullException(nameof(file));
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));

            Debug.Assert(!string.IsNullOrWhiteSpace(file.Name));
            Debug.Assert(authenticatedUser.Id != Guid.Empty);

            if (!Guid.TryParse(file.Name, out _)) return default;

            var sb = new StringBuilder();

            // Awful query to try and read, but in essence I am looking for a file and then assuring:
            //
            // 1. the user is an approved member of the site (e.Id IS NOT NULL)
            // 2. that the containing group or folder have not been deleted (c.IsDeleted and d.IsDeleted = 0),
            // 3. the user is an approved member of the group or a platform admin or the group is public
            //
            // Would prefer for the 'permissions/access' check to be outside of this query and part of a separate service

            sb.AppendLine($"SELECT      [{nameof(UserFileMetadata.FileId)}]                = a.[Id]");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.Title)}]                 = a.[Title]");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.Description)}]           = a.[Description]");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.GroupName)}]             = d.[Name]");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.Name)}]                  = a.[FileName]");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.FileVersion)}]           = @FileVersion");            
            sb.AppendLine($"          , [{nameof(UserFileMetadata.SizeInBytes)}]           = a.[FileSizeBytes]"); 
            sb.AppendLine($"          , [{nameof(UserFileMetadata.Extension)}]             = a.[FileExtension]");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.BlobName)}]              = a.[BlobName]");     
            sb.AppendLine($"          , [{nameof(UserFileMetadata.ContentHash)}]           = a.[BlobHash]"); 
            sb.AppendLine($"          , [{nameof(UserFileMetadata.LastWriteTimeUtc)}]      = CONVERT(DATETIMEOFFSET, ISNULL(a.[ModifiedAtUtc], a.[CreatedAtUtc]))");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.OwnerUserName)}]         = b.[UserName]");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.UserHasViewPermission)}] = IIF(e.[Id] IS NOT NULL, 1,0)");
            sb.AppendLine($"          , [{nameof(UserFileMetadata.UserHasEditPermission)}] = 0");
            sb.AppendLine($"FROM      dbo.[File]           a");
            sb.AppendLine($"JOIN      dbo.[MembershipUser] b ON b.[Id] = a.[CreatedBy]");
            sb.AppendLine($"JOIN      dbo.[Folder]         c ON c.[Id] = a.[ParentFolder] AND c.[IsDeleted] = 0");
            sb.AppendLine($"JOIN      dbo.[Group]          d ON d.[Id] = c.[Group_Id]  AND d.[IsDeleted] = 0");
            sb.AppendLine($"JOIN      dbo.[FileStatus]     h ON a.[FileStatus] = h.[Id]");

            // File Permission Check - see above comments for the logic being applied

            sb.AppendLine($"LEFT JOIN dbo.[MembershipUser] e ON e.[Id] = @UserId AND e.[IsApproved] = 1");
            sb.AppendLine($"LEFT JOIN dbo.[GroupUser] f ON f.[Group_Id] = d.[Id] AND f.[MembershipUser_Id] = e.[Id] AND f.[Approved] = 1");
         
            sb.AppendLine($"WHERE  a.[Id]               = @FileId");
            sb.AppendLine($"AND    h.[Name]             = 'Verified'");
            sb.AppendLine($"AND    a.[IsDeleted]        = 0" + $"");
            //sb.AppendLine($"AND    (a.[FileVersion] = @FileVersion OR ...)");

            // TODO - If the file version parameter is null then we need to locate the current version of the file and return the metadata for it
            //        As versioning hasn't yet been implemented, we will need to return to this method and wire it up when we know how

            var parameters = new { FileId = file.Name, FileVersion = file.Version, UserId = authenticatedUser.Id };

            var userFileMetadata = await _azureSqlClient.GetRecord<UserFileMetadata>(sb.ToString(), parameters, cancellationToken);

            return userFileMetadata;
        }
    }
}
