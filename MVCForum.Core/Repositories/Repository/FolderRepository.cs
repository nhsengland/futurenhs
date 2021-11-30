namespace MvcForum.Core.Repositories.Repository
{
    using Dapper;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class FolderRepository : IFolderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FolderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<PaginatedList<FolderReadViewModel>> GetRootFoldersForGroupAsync(string groupSlug, int page = 1, int pageSize = 999, CancellationToken cancellationToken = default(CancellationToken))
        {            
            const string query =
                @"
                    SELECT
                      folders.Id AS FolderId,
                      folders.Name AS FolderName,
                      (
                        SELECT
                          COUNT(*)
                        FROM
                          [File]
                        WHERE
                          ParentFolder = folders.Id
                      ) AS FileCount
                    FROM
                      Folder folders
                      JOIN [Group] groups ON groups.Id = folders.ParentGroup
                    WHERE
                      groups.Slug = @GroupSlug
                      AND folders.ParentFolder IS NULL
                      AND folders.IsDeleted = 0
                    ORDER BY
                      folders.Name OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                    SELECT
                      COUNT(*)
                    FROM
                      Folder folders
                      JOIN [Group] groups ON groups.Id = folders.ParentGroup
                    WHERE
                      groups.Slug = @GroupSlug
                      AND folders.ParentFolder IS NULL
                      AND folders.IsDeleted = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize,
                GroupSlug = groupSlug
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            using (var multipleResults = await dbConnection.QueryMultipleAsync(commandDefinition))
            {
                var results = multipleResults.Read<FolderReadViewModel>();
                var totalCount = multipleResults.ReadFirst<int>();
                return new PaginatedList<FolderReadViewModel>(results.ToList(), totalCount, page, pageSize);
            }
        }

        public async Task<IEnumerable<BreadCrumbItem>> GenerateBreadcrumbTrailAsync(Guid folderId, CancellationToken cancellationToken)
        {
            const string query =
                @"
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

            var commandDefinition = new CommandDefinition(query, new
            {
                FolderId = folderId
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return (await dbConnection.QueryAsync<BreadCrumbItem>(commandDefinition)).Reverse();
            }
        }

        public async Task<PaginatedList<FolderReadViewModel>> GetChildFoldersForFolderAsync(Guid parentFolderId, int page = 1, int pageSize = 999, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string query =
                @"
                    SELECT
                      f.Id AS FolderId,
                      f.Name AS FolderName,
                      (
                        SELECT
                          COUNT(*)
                        FROM
                          [File]
                        WHERE
                          ParentFolder = f.Id
                      ) AS FileCount
                    FROM
                      Folder f
                    WHERE
                      f.ParentFolder = @ParentFolder
                      AND f.IsDeleted = 0
                    ORDER BY
                      f.Name OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                    SELECT
                      COUNT(*)
                    FROM
                      Folder
                    WHERE
                      ParentFolder = @ParentFolder
                      AND IsDeleted = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize,
                ParentFolder = parentFolderId
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            using (var multipleResults = await dbConnection.QueryMultipleAsync(commandDefinition))
            {
                var results = multipleResults.Read<FolderReadViewModel>();
                var totalCount = multipleResults.ReadFirst<int>();
                return new PaginatedList<FolderReadViewModel>(results.ToList(), totalCount, page, pageSize);
            }
        }

        public async Task<FolderReadViewModel> GetFolderAsync(Guid folderId, CancellationToken cancellationToken)
        {            
            const string query =
                @"
                    SELECT
                      Id AS FolderId,
                      Name AS FolderName,
                      Description,
                      FileCount,
				 	  ParentFolder AS ParentId
                    FROM
                      Folder folders
                    WHERE
                      folders.Id = @FolderId
					  AND folders.IsDeleted = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                FolderId = folderId
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.QueryFirstOrDefaultAsync<FolderReadViewModel>(commandDefinition);
            }
        }

        public async Task<bool> IsFolderNameValidAsync(string folderName, 
                                                       Guid? parentFolderId,
                                                       Guid parentGroupId,
                                                       CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT
                      CAST(COUNT(1) AS BIT)
                    FROM
                      Folder f
                    WHERE
                      f.Name = @FolderName
                      AND f.IsDeleted = 0
                      AND (f.ParentFolder = @ParentFolderId OR @ParentFolderId IS NULL)
                      AND f.ParentGroup = @ParentGroupId;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                FolderName = folderName.Trim(),
                ParentFolderId = parentFolderId,
                ParentGroupId = parentGroupId
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return !await dbConnection.QueryFirstOrDefaultAsync<bool>(commandDefinition);
            }
        }

        public async Task<bool> IsFolderIdValidAsync(Guid folderId,
                                                     CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT
                      CAST(COUNT(1) AS BIT)
                    FROM
                      Folder f
                    WHERE
                      f.Id = @FolderId
                      AND f.IsDeleted = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                FolderId = folderId
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return await dbConnection.QueryFirstOrDefaultAsync<bool>(commandDefinition);
            }
        }

        public async Task<bool> IsUserAdminAsync(string groupSlug, Guid userId, CancellationToken cancellationToken)
        {
            var (MembershipRole, GroupRole) = await GetUserRolesAsync(groupSlug, userId, cancellationToken);

            return MembershipRole?.ToLower() == "admin" || GroupRole?.ToLower() == "admin";
        }

		public async Task<bool> UserHasGroupAccessAsync(string groupSlug, Guid userId, CancellationToken cancellationToken)
        {
            var (MembershipRole, GroupRole) = await GetUserRolesAsync(groupSlug, userId, cancellationToken);

            return MembershipRole?.ToLower() == "admin" || GroupRole?.ToLower() == "admin" || GroupRole?.ToLower() == "standard members";
        }

        private async Task<(string MembershipRole, string GroupRole)> GetUserRolesAsync(string groupSlug, Guid userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) throw new ArgumentNullException(nameof(groupSlug));
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(groupSlug));

            const string query =
                @"
                    SELECT
                      rolename AS MemberRole
                    FROM
                      membershiprole
                      JOIN membershipusersinroles m ON m.roleidentifier = id
                    WHERE
                      m.useridentifier = @UserId;

                    SELECT
                      mr.rolename AS GroupRole
                    FROM
                      groupuser gu
                      JOIN membershiprole mr ON gu.membershiprole_id = mr.id
                      JOIN membershipusersinroles mur ON mur.useridentifier = gu.membershipuser_id
                      JOIN [group] g ON gu.group_id = g.id
                    WHERE
                      g.slug = @GroupSlug
                      AND gu.membershipuser_id = @UserId
                      AND gu.approved = 1
                      AND gu.banned = 0
                      AND gu.locked = 0;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                GroupSlug = groupSlug,
                UserId = userId
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            using (var result = await dbConnection.QueryMultipleAsync(commandDefinition))
            {
                var membershipRole = result.Read<string>().FirstOrDefault();
                var groupRole = result.Read<string>().FirstOrDefault();

                return (membershipRole, groupRole);
            }
        }
    }
}


