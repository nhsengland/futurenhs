using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.FileAndFolder;
using FutureNHS.Api.DataAccess.Models.User;
using Microsoft.AspNetCore.StaticFiles;
using File = FutureNHS.Api.DataAccess.Models.FileAndFolder.File;

namespace FutureNHS.Api.DataAccess.Database.Read
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
                @$" SELECT
                                [{nameof(FolderContentsData.Id)}]                   = folder.Id,
                                [{nameof(FolderContentsData.Type)}]                 = 'Folder', 
                                [{nameof(FolderContentsData.Name)}]                 = folder.Title, 
                                [{nameof(FolderContentsData.Description)}]          = folder.Description,
                                [{nameof(FolderContentsData.CreatedAtUtc)}]         = folder.CreatedAtUtc,
                                [{nameof(FolderContentsData.CreatedById)}]          = folder.CreatedBy,
                                [{nameof(FolderContentsData.CreatedByName)}]        = CreatedByUser.FirstName + ' ' + CreatedByUser.Surname,
                                [{nameof(FolderContentsData.CreatedBySlug)}]        = CreatedByUser.Slug,
                                [{nameof(FolderContentsData.ModifiedAtUtc)}]        = NULL,
                                [{nameof(FolderContentsData.ModifiedById)}]         = NULL,
                                [{nameof(FolderContentsData.ModifiedByName)}]       = NULL,
                                [{nameof(FolderContentsData.ModifiedBySlug)}]       = NULL,
                                [{nameof(FolderContentsData.ModifiedBySlug)}]       = NULL,
                                [{nameof(FolderContentsData.FileExtension)}]        = NULL

                    FROM        Folder folder
                    LEFT JOIN   MembershipUser CreatedByUser 
                    ON          CreatedByUser.Id = folder.CreatedBy
                    JOIN        [Group] groups 
                    ON          groups.Id = folder.Group_Id
                    WHERE       groups.Slug = @Slug 
                    AND         folder.ParentFolder IS NULL 
                    AND         folder.IsDeleted = 0
                    ORDER BY    Name

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 

                    FROM        Folder folder
                    JOIN        [Group] groups 
                    ON          groups.Id = folder.Group_Id
                    WHERE       groups.Slug = @Slug 
                    AND         folder.ParentFolder IS NULL 
                    AND         folder.IsDeleted = 0";

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
                @$" SELECT
                                [{nameof(Folder.Id)}]                               = folders.Id,
                                [{nameof(Folder.Name)}]                             = folders.Title,
                                [{nameof(Folder.Description)}]                      = folders.Description,
                                [{nameof(Models.Shared.Properties.AtUtc)}]          = folders.CreatedAtUtc,
                                [{nameof(UserNavProperty.Id)}]                      = CreatedByUser.Id,
                                [{nameof(UserNavProperty.Name)}]                    = CreatedByUser.FirstName + ' ' + CreatedByUser.Surname,
                                [{nameof(UserNavProperty.Slug)}]                    = CreatedByUser.Slug

                    FROM        Folder folders
                    JOIN        MembershipUser CreatedByUser 
                    ON          CreatedByUser.Id = folders.CreatedBy
                    WHERE       folders.Id = @FolderId
                    AND         folders.IsDeleted = 0;

                    WITH BreadCrumbs AS 
                    (
                    SELECT      
                                Id,
                                Title,
                                ParentFolder

                    FROM        Folder
                    WHERE       Id = @FolderId
                    UNION ALL
                    SELECT
                                folder.Id AS PK,
                                folder.[Title] AS Title,
                                folder.ParentFolder AS ParentFK

                    FROM        Folder folder
                    INNER JOIN  BreadCrumbs Breadcrumb 
                    ON          Breadcrumb.ParentFolder = folder.Id
                    )
                    SELECT
                                [{nameof(FolderPathItem.Id)}]    = Id,
                                [{nameof(FolderPathItem.Name)}]  = Title
                    FROM        BreadCrumbs;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            using var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                FolderId = folderId
            });

            var folders = reader.Read<Folder, Models.Shared.Properties, UserNavProperty, Folder>(
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

                }, splitOn: $"{nameof(Models.Shared.Properties.AtUtc)}, {nameof(Folder.Id)}");

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
                @$"SELECT
                                [{nameof(FolderContentsData.Id)}]               = folder.Id,
                                [{nameof(FolderContentsData.Type)}]             = 'Folder', 
                                [{nameof(FolderContentsData.Name)}]             = folder.Title, 
                                [{nameof(FolderContentsData.Description)}]      = folder.Description,
                                [{nameof(FolderContentsData.CreatedAtUtc)}]     = folder.CreatedAtUtc, 
                                [{nameof(FolderContentsData.CreatedById)}]      = folder.CreatedBy,
                                [{nameof(FolderContentsData.CreatedByName)}]    = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(FolderContentsData.CreatedBySlug)}]    = createUser.Slug,
                                [{nameof(FolderContentsData.ModifiedAtUtc)}]    = null,
                                [{nameof(FolderContentsData.ModifiedById)}]     = null,
                                [{nameof(FolderContentsData.ModifiedByName)}]   = null,
                                [{nameof(FolderContentsData.ModifiedBySlug)}]   = null,
                                [{nameof(FolderContentsData.ModifiedBySlug)}]   = null,
                                [{nameof(FolderContentsData.FileExtension)}]    = null

                    FROM        Folder folder
                    LEFT JOIN   MembershipUser CreateUser 
                    ON          CreateUser.Id = folder.CreatedBy
                    WHERE       ParentFolder = @FolderId 
                    AND         folder.IsDeleted = 0
                    UNION ALL
                    SELECT 
                                files.Id,
                                'File',
                                Title, 
                                Description,
                                files.CreatedAtUtc, 
                                CreatedBy,
                                CreateUser.FirstName + ' ' + createUser.Surname,
                                CreateUser.Slug, 
                                files.ModifiedAtUtc,
                                ModifiedBy,
                                ModifyUser.FirstName + ' ' + createUser.Surname,
                                ModifyUser.Slug,
                                FileName,
                                FileExtension

                    FROM        [File] files
                    LEFT JOIN   MembershipUser CreateUser 
                    ON          CreateUser.Id = files.CreatedBy
                    LEFT JOIN   MembershipUser ModifyUser 
                    ON          ModifyUser.Id = files.ModifiedBy
                    WHERE       ParentFolder = @FolderId 
                    AND         FileStatus = (SELECT Id FROM [FileStatus] WHERE Name = 'Verified')
                    ORDER BY    type DESC, Name

                    OFFSET @Offset ROWS
                    FETCH NEXT @Limit ROWS ONLY;

                    SELECT SUM(items) 
                    FROM
                    (
                    SELECT      COUNT(*) 

                    AS          items 
                    FROM        Folder folder
                    WHERE       ParentFolder = @FolderId 
                    AND         IsDeleted = 0
                    UNION ALL
                    SELECT      COUNT(*) 

                    FROM        [File] files
                    WHERE       ParentFolder = @FolderId 
                    AND         FileStatus  = (SELECT Id FROM [FileStatus] WHERE Name = 'Verified')
                    ) 
                    AS          TOTAL";

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
                @$"SELECT
                                [{nameof(FileData.Id)}]             = files.Id, 
                                [{nameof(FileData.Title)}]          = files.Title, 
                                [{nameof(FileData.Description)}]    = files.Description, 
                                [{nameof(FileData.CreatedAtUtc)}]   = files.CreatedAtUtc,
                                [{nameof(FileData.CreatorId)}]      = createUser.Id,
                                [{nameof(FileData.CreatorName)}]    = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(FileData.CreatorSlug)}]    = createUser.Slug,  
                                [{nameof(FileData.ModifiedAtUtc)}]  = files.ModifiedAtUtc,
                                [{nameof(FileData.ModifierId)}]     = modifyUser.Id,
                                [{nameof(FileData.ModifierName)}]   = modifyUser.FirstName + ' ' + modifyUser.Surname,
                                [{nameof(FileData.ModifierSlug)}]   = modifyUser.Slug,
				                [{nameof(FileData.FileName)}]       = files.FileName,
				                [{nameof(FileData.FileExtension)}]  = files.FileExtension

                    FROM        [File] files
                    LEFT JOIN   MembershipUser createUser 
                    ON          createUser.Id = files.CreatedBy
					LEFT JOIN   MembershipUser modifyUser 
                    ON          modifyUser.Id = files.ModifiedBy
                    WHERE       files.Id = @FileId 
                    AND         FileStatus = (SELECT Id FROM [FileStatus] WHERE Name = 'Verified');                    
                    WITH BreadCrumbs
                    AS 
                    (
                    SELECT
                                folder.Id,
                                folder.Title,
                                folder.ParentFolder AS ParentFolder

                    FROM        Folder folder
                    JOIN        [File] files 
                    ON          files.ParentFolder = folder.Id
                    WHERE       files.Id = @FileId
                    UNION ALL
                    SELECT
                                PK = folder.Id,
                                Title = folder.[Title],
                                ParentFK = folder.ParentFolder

                    FROM        Folder folder
                    INNER JOIN  BreadCrumbs Breadcrumb 
                    ON          Breadcrumb.ParentFolder = folder.Id
                    )
                    
                    SELECT
                                [{nameof(FolderPathItem.Id)}]    = Id,
                                [{nameof(FolderPathItem.Name)}]  = Title

                    FROM        BreadCrumbs;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            using var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                FileId = fileId
            });

            var fileData = await reader.ReadFirstOrDefaultAsync<FileData>();
            var pathToFile = await reader.ReadAsync<FolderPathItem>();

            if (fileData is null)
                return null;
            
            const string versionQuery =
                $@"SELECT
                            [{nameof(FileData.Id)}]             = fh.Id, 
                            [{nameof(FileData.Title)}]          = fh.Title,
                            [{nameof(FileData.Description)}]    = fh.Description, 
                            [{nameof(FileData.CreatedAtUtc)}]   = files.CreatedAtUtc,
                            [{nameof(FileData.Size)}]           = fh.FileSizeBytes,
                            [{nameof(FileData.ModifiedAtUtc)}]  = fh.ModifiedAtUtc,
                            [{nameof(FileData.ModifierId)}]     = mu.Id,
                            [{nameof(FileData.ModifierName)}]   = mu.FirstName + ' ' + mu.Surname,
                            [{nameof(FileData.FileName)}]       = fh.FileName,
				            [{nameof(FileData.FileExtension)}]  = fh.FileExtension
                            
                                
                FROM        FileHistory fh
                LEFT JOIN   MembershipUser mu
                ON          mu.Id = fh.ModifiedBy
                RIGHT JOIN  [File] files
                ON          files.Id = fh.fileId
                WHERE       fh.FileId = @FileId
                ORDER BY    fh.ModifiedAtUtc DESC;";
            
            using var fileHistoryDbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            using var versionReader = await dbConnection.QueryMultipleAsync(versionQuery, new
            {
                FileId = fileId
            });

            List<FileData>versions = versionReader.Read<FileData>().ToList();
            var fileModel =  GenerateFileModelFromData(fileData, pathToFile, versions);

            return fileModel;

        }

        private File GenerateFileModelFromData(FileData fileData, IEnumerable<FolderPathItem> pathToFile,List<FileData> versions)
        {
            new FileExtensionContentTypeProvider().Mappings.TryGetValue(fileData.FileExtension, out var mimeType);

            var file = new File
            {
                Id = fileData.Id,
                Name = fileData.Title,
                Description = fileData.Description,
                FirstRegistered = new Models.Shared.Properties
                {
                    AtUtc = fileData.CreatedAtUtc,
                    By = new UserNavProperty
                    {
                        Id = fileData.CreatorId,
                        Name = fileData.CreatorName,
                        Slug = fileData.CreatorSlug
                    }
                },
                Versions = versions.Select(version => new FileVersion
                {
                    Id = version.Id,
                    Name = version.FileName,
                    Size = version.Size,
                    LastUpdated = new Models.Shared.Properties
                    {
                        AtUtc = version.ModifiedAtUtc,
                        By = new UserNavProperty
                        {
                            Id = version.ModifierId.GetValueOrDefault(),
                            Name = version.ModifierName,
                            Slug = version.ModifierSlug
                        }
                    }
    
                }).ToList(),
                Path = pathToFile
            };

            if (fileData.ModifiedAtUtc is not null)
            {
                file = file with
                {
                    LastUpdated = new Models.Shared.Properties
                    {
                        AtUtc = fileData.ModifiedAtUtc,
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
                    FirstRegistered = new Models.Shared.Properties
                    {
                        AtUtc = item.CreatedAtUtc,
                        By = new UserNavProperty
                        {
                            Id = item.CreatedById,
                            Name = item.CreatedByName,
                            Slug = item.CreatedBySlug
                        }
                    }
                };

                if (item.ModifiedAtUtc is not null)
                {
                    file = file with
                    {
                        LastUpdated = new Models.Shared.Properties
                        {
                            AtUtc = item.ModifiedAtUtc,
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
