using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.FileAndFolder;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using File = FutureNHS.Api.DataAccess.Models.FileAndFolder.File;

namespace FutureNHS.Api.DataAccess.Repositories.Read
{
    public class FileAndFolderDataProvider : IFileAndFolderDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<FileAndFolderDataProvider> _logger;

        public FileAndFolderDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<FileAndFolderDataProvider> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }
        public async Task<(uint total, IEnumerable<FolderContentsItem>?)> GetRootFoldersAsync(string groupSlug, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @"
                   SELECT
                    folder.Id AS Id,
                    'Folder' AS type, 
                    folder.Name AS Name, 
                    folder.Description AS Description,
                    folder.CreatedAtUtc AS CreatedAtUtc, 
                    folder.AddedBy AS CreatedById,
                    createUser.FirstName + ' ' + createUser.Surname AS CreatedByName,
                    createUser.Slug AS CreatedBySlug,
                    null AS ModifiedAtUtc,
                    null AS ModifiedById,
                    null AS ModifiedByName,
                    null AS ModifiedBySlug,
                    null AS FileName,
                    null AS FileExtension
                   FROM Folder folder
                    LEFT JOIN MembershipUser CreateUser ON CreateUser.Id = folder.AddedBy
                    JOIN [Group] groups on groups.Id = folder.ParentGroup
                   WHERE groups.Slug = @Slug AND folder.ParentFolder IS NULL AND folder.IsDeleted = 0
                   ORDER BY Name
                   OFFSET @Offset ROWS
                   FETCH NEXT @Limit ROWS ONLY;

                   SELECT COUNT(*) FROM Folder folder
                   JOIN [Group] groups on groups.Id = folder.ParentGroup
                   WHERE groups.Slug = @Slug AND folder.ParentFolder IS NULL AND folder.IsDeleted = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Slug = groupSlug
            });

            var contents = await reader.ReadAsync<FolderContentsData>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, GenerateContentsModelFromData(contents));
        }

        public async Task<Folder?> GetFolderAsync(Guid folderId, CancellationToken cancellationToken)
        {
            const string query =
                @"
                           SELECT
                            folders.Id AS Id,
                            Name AS Name,
                            Description,
                            folders.CreatedAtUtc AS ActionAtUtc,
                            mu.Id AS Id,
                            mu.FirstName + ' ' + mu.Surname AS Name,
                            mu.Slug AS Slug                         
                           FROM
                            Folder folders
                            JOIN MembershipUser mu ON mu.Id = folders.AddedBy
                           WHERE
                             folders.Id = @FolderId
                             AND folders.IsDeleted = 0;

                           WITH BreadCrumbs AS (
                             SELECT
                               Id,
                               Name,
                               ParentFolder AS ParentFolder
                             FROM
                               Folder
                             WHERE
                               Id = @FolderId
                             UNION ALL
                             SELECT
                               F.Id AS PK,
                               F.[Name] AS Name,
                               F.ParentFolder AS ParentFK
                             FROM
                               Folder F
                               INNER JOIN BreadCrumbs BC ON BC.ParentFolder = F.Id
                           )
                           SELECT
                             Id,
                             Name
                           FROM
                             BreadCrumbs;
                       ";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            using var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                FolderId = folderId
            });

            var folders = reader.Read<Folder, Models.FileAndFolder.Properties, UserNavProperty, Folder>(
                (folderDetails, folderProperties, userNavProperty) =>
                {
                    if (folderProperties is not null)
                    {
                        if (userNavProperty is not null)
                        {
                            var folderWithUserInfo = folderDetails with { FirstRegistered = folderProperties with { By = userNavProperty } };
                            return folderWithUserInfo;
                        }
                        folderDetails = folderDetails with { FirstRegistered = folderProperties };
                        return folderDetails;
                    }

                    return folderDetails;

                }, splitOn: "ActionAtUtc, id");

            var folder = folders.FirstOrDefault();

            if (folder != null)
            {
                folder = folder with { Path = await reader.ReadAsync<FolderPathItem>() };
                return folder;
            }

            return folder;
        }


        public async Task<(uint total, IEnumerable<FolderContentsItem>?)> GetFolderContentsAsync(Guid folderId, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @"
                   SELECT
                    folder.Id AS Id,
                    'Folder' AS type, 
                    Name AS Name, 
                    Description AS Description,
                    CreatedAtUtc AS CreatedAtUtc, 
                    AddedBy AS CreatedById,
                    createUser.FirstName + ' ' + createUser.Surname AS CreatedByName,
                    createUser.Slug AS CreatedBySlug,
                    null AS ModifiedAtUtc,
                    null AS ModifiedById,
                    null AS ModifiedByName,
                    null AS ModifiedBySlug,
                    null AS FileName,
                    null AS FileExtension
                   FROM Folder folder
                    LEFT JOIN MembershipUser CreateUser ON CreateUser.Id = folder.AddedBy
                   WHERE ParentFolder = @FolderId AND IsDeleted = 0
                   UNION ALL
                   SELECT 
                    files.Id,
                    'File',
                    Title, 
                    Description,
                    CreatedAtUtc, 
                    CreatedBy,
                    CreateUser.FirstName + ' ' + createUser.Surname,
                    CreateUser.Slug, 
                    ModifiedAtUtc, 
                    ModifiedBy,
                    ModifyUser.FirstName + ' ' + createUser.Surname,
                    ModifyUser.Slug,
                    FileName,
                    FileExtension
                   FROM [File] files
                    LEFT JOIN MembershipUser CreateUser ON CreateUser.Id = files.CreatedBy
                    LEFT JOIN MembershipUser ModifyUser ON ModifyUser.Id = files.ModifiedBy
                   WHERE ParentFolder = @FolderId AND FileStatus = 4
                   ORDER BY type DESC, Name
                   OFFSET @Offset ROWS
                   FETCH NEXT @Limit ROWS ONLY;

                SELECT SUM(items) FROM
                (
                 SELECT COUNT(*) AS items FROM Folder folder
                 WHERE ParentFolder = @FolderId AND IsDeleted = 0
                 UNION ALL
                 SELECT COUNT(*) FROM [File] files
                 WHERE ParentFolder = @FolderId AND FileStatus = 4
                ) AS TOTAL"; 

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                FolderId = folderId
            });

            var contents = await reader.ReadAsync<FolderContentsData>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, GenerateContentsModelFromData(contents));
        }

        public async Task<File?> GetFileAsync(Guid fileId, CancellationToken cancellationToken)
        {
            const string query =
                @"
                         SELECT
                            files.Id AS Id, 
                            files.Title AS Title, 
                            files.Description AS Description,                           
                            files.CreatedAtUtc AS CreatedAtUtc,
                            createUser.Id AS CreatorId,
                            createUser.FirstName + ' ' + createUser.Surname AS CreatorName,
                            createUser.Slug AS CreatorSlug,  
                            files.ModifiedAtUtc AS ModifiedAtUtc,
                            modifyUser.Id AS ModifierId,
                            modifyUser.FirstName + ' ' + modifyUser.Surname AS ModifierName,
                            modifyUser.Slug AS ModifierSlug,
							files.Id AS VersionId,
							files.FileName As FileName,
							files.FileExtension AS FileExtension			
                           FROM
                            [File] files
                            LEFT JOIN MembershipUser createUser ON createUser.Id = files.CreatedBy
							LEFT JOIN MembershipUser modifyUser ON modifyUser.Id = files.ModifiedBy
                           WHERE
                             files.Id = @FileId AND FileStatus = 4;

                           WITH BreadCrumbs AS (
                             SELECT
                               folder.Id,
                               folder.Name,
                               folder.ParentFolder AS ParentFolder
                             FROM
                               Folder folder
                            JOIN [File] files on files.ParentFolder = folder.Id
                             WHERE
                               files.Id = @FileId
                             UNION ALL
                             SELECT
                               F.Id AS PK,
                               F.[Name] AS Name,
                               F.ParentFolder AS ParentFK
                             FROM
                               Folder F
                               INNER JOIN BreadCrumbs BC ON BC.ParentFolder = F.Id
                           )
                           SELECT
                             Id,
                             Name
                           FROM
                             BreadCrumbs;
                       ";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            using var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                FileId = fileId
            });

            var fileData = await reader.ReadFirstOrDefaultAsync<FileData>();
            var pathToFile = await reader.ReadAsync<FolderPathItem>();

            if (fileData is null)
                return null;

            return GenerateFileModelFromData(fileData, pathToFile);
        }

        private File GenerateFileModelFromData(FileData fileData, IEnumerable<FolderPathItem> pathToFile)
        { 
            new FileExtensionContentTypeProvider().Mappings.TryGetValue(fileData.FileExtension, out var mimeType);

            var file = new File
            {
                Id = fileData.Id,
                Name = fileData.Title,
                Description = fileData.Description,
                FirstRegistered = new Models.FileAndFolder.Properties
                {
                    AtUtc = fileData.CreatedAtUtc?.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                    By = new UserNavProperty
                    {
                        Id = fileData.CreatorId,
                        Name = fileData.CreatorName,
                        Slug = fileData.CreatorSlug
                    }
                },
                Versions = new FileVersion
                {
                    Id = fileData.Id,
                    Name = fileData.FileName,
                    FirstRegistered = new Models.FileAndFolder.Properties
                    {
                        AtUtc = fileData.CreatedAtUtc?.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                        By = new UserNavProperty
                        {
                            Id = fileData.CreatorId,
                            Name = fileData.CreatorName,
                            Slug = fileData.CreatorSlug
                        }
                    },
                    AdditionalMetadata = new FileProperties
                    {
                        FileExtension = fileData.FileExtension,
                        MediaType = mimeType
                    }
                },
                Path = pathToFile
            };

            if (fileData.ModifiedAtUtc.HasValue)
            {
                file = file with
                {
                    LastUpdated = new Models.FileAndFolder.Properties
                    {
                        AtUtc = fileData.ModifiedAtUtc?.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                        By = new UserNavProperty
                        {
                            Id = fileData.ModifierId.GetValueOrDefault(),
                            Name = fileData.ModifierName,
                            Slug = fileData.ModifierSlug
                        }
                    }
                };
            }
            return file;
        }

        private IEnumerable<FolderContentsItem> GenerateContentsModelFromData(IEnumerable<FolderContentsData> contentData)
        {
            var contentItems = new List<FolderContentsItem>();

            foreach (var item in contentData)
            {
                var file = new FolderContentsItem()
                {
                    Id = item.Id,
                    Type = item.Type,
                    Name = item.Name,
                    Description = item.Description,
                    FirstRegistered = new Models.FileAndFolder.Properties
                    {
                        AtUtc = item.CreatedAtUtc?.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                        By = new UserNavProperty
                        {
                            Id = item.CreatedById,
                            Name = item.CreatedByName,
                            Slug = item.CreatedBySlug
                        }
                    }
                };

                if (item.ModifiedAtUtc.HasValue)
                {
                    file = file with
                    {
                        LastUpdated = new Models.FileAndFolder.Properties
                        {
                            AtUtc = item.ModifiedAtUtc?.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                            By = new UserNavProperty
                            {
                                Id = item.ModifiedById.GetValueOrDefault(),
                                Name = item.ModifiedByName,
                                Slug = item.ModifiedBySlug
                            }
                        }
                    };
                }

                if (item.FileExtension is not null)
                {
                    new FileExtensionContentTypeProvider().Mappings.TryGetValue(item.FileExtension, out var mimeType);

                    file = file with
                    {
                        AdditionalMetadata = new FileProperties
                        {
                            FileExtension = item.FileExtension,
                            MediaType = mimeType
                        }
                    };
                }
                contentItems.Add(file);
            }

            return contentItems;
        }
    }
}
