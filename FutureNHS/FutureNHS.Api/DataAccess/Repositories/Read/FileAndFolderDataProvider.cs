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
                @$" SELECT
                                [{nameof(FolderContentsData.Id)}]                   = folder.Id,
                                [{nameof(FolderContentsData.Type)}]                 = 'Folder', 
                                [{nameof(FolderContentsData.Name)}]                 = folder.Name, 
                                [{nameof(FolderContentsData.Description)}]          = folder.Description,
                                [{nameof(FolderContentsData.CreatedAtUtc)}]         = FORMAT(folder.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(FolderContentsData.CreatedById)}]          = folder.AddedBy,
                                [{nameof(FolderContentsData.CreatedByName)}]        = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(FolderContentsData.CreatedBySlug)}]        = createUser.Slug,
                                [{nameof(FolderContentsData.ModifiedAtUtc)}]        = NULL,
                                [{nameof(FolderContentsData.ModifiedById)}]         = NULL,
                                [{nameof(FolderContentsData.ModifiedByName)}]       = NULL,
                                [{nameof(FolderContentsData.ModifiedBySlug)}]       = NULL,
                                [{nameof(FolderContentsData.ModifiedBySlug)}]       = NULL,
                                [{nameof(FolderContentsData.FileExtension)}]        = NULL
                    FROM        Folder folder
                    LEFT JOIN   MembershipUser CreateUser 
                    ON          CreateUser.Id = folder.AddedBy
                    JOIN        [Group] groups 
                    ON          groups.Id = folder.ParentGroup
                    WHERE       groups.Slug = @Slug 
                    AND         folder.ParentFolder IS NULL 
                    AND         folder.IsDeleted = 0
                    ORDER BY    Name

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 

                    FROM        Folder folder
                    JOIN        [Group] groups 
                    ON          groups.Id = folder.ParentGroup
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
                                [{nameof(Folder.Name)}]                             = folders.Name,
                                [{nameof(Folder.Description)}]                      = folders.Description,
                                [{nameof(Models.Shared.Properties.AtUtc)}]   = FORMAT(folders.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(UserNavProperty.Id)}]                      = mu.Id,
                                [{nameof(UserNavProperty.Name)}]                    = mu.FirstName + ' ' + mu.Surname,
                                [{nameof(UserNavProperty.Slug)}]                    = mu.Slug  

                    FROM        Folder folders
                    JOIN        MembershipUser mu 
                    ON          mu.Id = folders.AddedBy
                    WHERE       folders.Id = @FolderId
                    AND         folders.IsDeleted = 0;

                    WITH BreadCrumbs AS 
                    (
                    SELECT      
                                Id,
                                Name,
                                ParentFolder

                    FROM        Folder
                    WHERE       Id = @FolderId
                    UNION ALL
                    SELECT
                                folder.Id AS PK,
                                folder.[Name] AS Name,
                                folder.ParentFolder AS ParentFK

                    FROM        Folder folder
                    INNER JOIN  BreadCrumbs Breadcrumb 
                    ON          Breadcrumb.ParentFolder = folder.Id
                    )
                    SELECT
                                [{nameof(FolderPathItem.Id)}]    = Id,
                                [{nameof(FolderPathItem.Name)}]  = Name
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
                            var folderWithUserInfo = folderDetails with { FirstRegistered = folderProperties with {By = userNavProperty } };
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
                                [{nameof(FolderContentsData.Name)}]             = folder.Name, 
                                [{nameof(FolderContentsData.Description)}]      = folder.Description,
                                [{nameof(FolderContentsData.CreatedAtUtc)}]     = FORMAT(folder.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'), 
                                [{nameof(FolderContentsData.CreatedById)}]      = folder.AddedBy,
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
                    ON          CreateUser.Id = folder.AddedBy
                    WHERE       ParentFolder = @FolderId 
                    AND         IsDeleted = 0
                    UNION ALL
                    SELECT 
                                files.Id,
                                'File',
                                Title, 
                                Description,
                                FORMAT(files.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'), 
                                CreatedBy,
                                CreateUser.FirstName + ' ' + createUser.Surname,
                                CreateUser.Slug, 
                                FORMAT(files.ModifiedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
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
                    AND         FileStatus = 4
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
                    AND         FileStatus = 4
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
                                [{nameof(FileData.CreatedAtUtc)}]   = FORMAT(files.CreatedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(FileData.CreatorId)}]      = createUser.Id,
                                [{nameof(FileData.CreatorName)}]    = createUser.FirstName + ' ' + createUser.Surname,
                                [{nameof(FileData.CreatorSlug)}]    = createUser.Slug,  
                                [{nameof(FileData.ModifiedAtUtc)}]  = FORMAT(files.ModifiedAtUtc,'yyyy-MM-ddTHH:mm:ssZ'),
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
                    AND         FileStatus = 4;                    
                    WITH BreadCrumbs
                    AS 
                    (
                    SELECT
                                folder.Id,
                                folder.Name,
                                folder.ParentFolder AS ParentFolder

                    FROM        Folder folder
                    JOIN        [File] files 
                    ON          files.ParentFolder = folder.Id
                    WHERE       files.Id = @FileId
                    UNION ALL
                    SELECT
                                PK = folder.Id,
                                Name = folder.[Name],
                                ParentFK = folder.ParentFolder

                    FROM        Folder folder
                    INNER JOIN  BreadCrumbs Breadcrumb 
                    ON          Breadcrumb.ParentFolder = folder.Id
                    )
                    
                    SELECT
                                [{nameof(FolderPathItem.Id)}]    = Id,
                                [{nameof(FolderPathItem.Name)}]  = Name

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
                Versions = new FileVersion
                {
                    Id = fileData.Id,
                    Name = fileData.FileName,
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
                    AdditionalMetadata = new FileProperties
                    {
                        FileExtension = fileData.FileExtension,
                        MediaType = mimeType
                    }
                },
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
