namespace MvcForum.Core.Repositories.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;

    public class FolderRepository : IFolderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FolderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public PaginatedList<FolderReadViewModel> GetRootFoldersForGroup(string groupSlug, int page = 1, int pageSize = 999)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            PaginatedList<FolderReadViewModel> folders;
            const string query =
                @"SELECT    folders.Id AS FolderId, 
                            folders.Name AS FolderName, 
                            (
				                SELECT	COUNT(*)
    				            FROM	[File]
    				            WHERE	ParentFolder = folders.Id
    			            ) AS FileCount
                FROM        Folder folders
                JOIN        [Group] groups 
                    ON      groups.Id = folders.ParentGroup
                WHERE       groups.Slug = @GroupSlug 
                AND         folders.ParentFolder IS NULL 
                AND         folders.IsDeleted = 0
                ORDER BY    folders.Name
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM Folder folders
                JOIN [Group] groups ON groups.Id = folders.ParentGroup
                WHERE groups.Slug = @GroupSlug
                AND folders.ParentFolder IS NULL AND folders.IsDeleted = 0";

            using (var multipleResults = dbConnection.QueryMultiple(query, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize,
                GroupSlug = groupSlug
            }))
            {

                var results = multipleResults.Read<FolderReadViewModel>();
                var totalCount = multipleResults.ReadFirst<int>();
                folders = new PaginatedList<FolderReadViewModel>(results.ToList(), totalCount, page, pageSize);
            }

            return folders;
        }

        public IEnumerable<BreadCrumbItem> GenerateBreadcrumbTrail(Guid folderId)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @";WITH BreadCrumbs
                AS
                (
                SELECT Id, Name, ParentFolder AS ParentFolder
                FROM Folder
                WHERE Id = @FolderId
                UNION ALL
                SELECT F.Id AS PK, F.[Name] AS Name, F.ParentFolder AS ParentFK
                FROM Folder F
                INNER JOIN BreadCrumbs BC
                    ON BC.ParentFolder = F.Id
                )
                SELECT Id, Name FROM BreadCrumbs";

            using (var multipleResults = dbConnection.QueryMultiple(query, new
            {
                FolderId = folderId,
            }))
            {

                var results = multipleResults.Read<BreadCrumbItem>().Reverse();
                return results;
            }
        }

        public PaginatedList<FolderReadViewModel> GetChildFoldersForFolder(Guid parentFolderId, int page = 1, int pageSize = 999)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            PaginatedList<FolderReadViewModel> folders;
            const string query =
                @"SELECT    f.Id AS FolderId, 
                            f.Name AS FolderName, 
                            (
				                SELECT	COUNT(*)
    				            FROM	[File]
    				            WHERE	ParentFolder = f.Id
    			            ) AS FileCount
                FROM        Folder f
                WHERE       f.ParentFolder = @ParentFolder 
                AND         f.IsDeleted = 0
                ORDER BY    f.Name
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM Folder
                WHERE ParentFolder = @ParentFolder  
                AND IsDeleted = 0";

            using (var multipleResults = dbConnection.QueryMultiple(query, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize,
                ParentFolder = parentFolderId
            }))
            {

                var results = multipleResults.Read<FolderReadViewModel>();
                var totalCount = multipleResults.ReadFirst<int>();
                folders = new PaginatedList<FolderReadViewModel>(results.ToList(), totalCount, page, pageSize);
            }

            return folders;
        }

        public FolderReadViewModel GetFolder(Guid folderId)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            
            const string query =
                @"SELECT Id AS FolderId, Name AS FolderName, Description, FileCount 
                FROM Folder folders
                WHERE folders.Id = @FolderId
                AND folders.IsDeleted = 0";

            var result = dbConnection.QueryFirstOrDefault<FolderReadViewModel>(query, new {FolderId = folderId});
            return result;
        }

        /// <summary>
        /// Get folder by Id, folder name and parent - validate folder exists for create/update, i.e. no duplicate names allowed.
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="folderName"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        public FolderReadViewModel GetFolder(Guid? folderId, string folderName, Guid? parentFolder)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"SELECT    Id AS FolderId, 
                            Name AS FolderName, 
                            Description, 
                            FileCount 
                FROM        Folder f
                WHERE       (
                                f.Id != @FolderId
                                OR
                                @FolderId IS NULL
                            )
                AND         f.Name = @FolderName
                AND         f.IsDeleted = 0
                AND         (
                                ParentFolder = @ParentFolder
                                OR
                                @ParentFolder IS NULL
                            );";

            var result = dbConnection.QueryFirstOrDefault<FolderReadViewModel>(query, new { FolderId = folderId, FolderName = folderName, ParentFolder = parentFolder });
            return result;
        }

        public bool UserIsAdmin(string groupSlug, Guid userId)
        {
            var (MembershipRole, GroupRole) = GetUserRoles(groupSlug, userId);

            return MembershipRole?.ToLower() == "admin" || GroupRole?.ToLower() == "admin";
        }

        public bool UserHasGroupAccess(string groupSlug, Guid userId)
        {
            var (MembershipRole, GroupRole) = GetUserRoles(groupSlug, userId);

            return MembershipRole?.ToLower() == "admin" || GroupRole?.ToLower() == "admin" || GroupRole?.ToLower() == "standard members";
        }

        private (string MembershipRole, string GroupRole) GetUserRoles(string groupSlug, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) throw new ArgumentNullException(nameof(groupSlug));
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(groupSlug));

            var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            const string query =
                @"
                    SELECT rolename AS MemberRole
                    FROM   membershiprole
                           JOIN membershipusersinroles m
                             ON m.roleidentifier = id
                    WHERE  m.useridentifier = @userId

                    SELECT mr.rolename AS GroupRole
                    FROM   groupuser gu
                           JOIN membershiprole mr
                             ON gu.membershiprole_id = mr.id
                           JOIN membershipusersinroles mur
                             ON mur.useridentifier = gu.membershipuser_id
                           JOIN [group] g
                             ON gu.group_id = g.id
                    WHERE  g.slug = @groupSlug
                           AND gu.membershipuser_id = @userId
                           AND gu.approved = 1
                           AND gu.banned = 0
                           AND gu.locked = 0
                ";

            using (var result = dbConnection.QueryMultiple(query, new { groupSlug, userId }))
            {
                var membershipRole = result.Read<string>().FirstOrDefault();
                var groupRole = result.Read<string>().FirstOrDefault();

                return (membershipRole, groupRole);
            }
        }

        //public async Task<PaginatedList<Folder>> GetFolders(int page = 1, int pageSize = 10)
        //{
        //    var dbConnection = _connectionFactory.CreateReadOnlyConnection();
        //    PaginatedList<Folder> groupsList;
        //    const string query =
        //    @"SELECT groups.Name, groups.Description, groups.Slug FROM [group] groups
        // WHERE groups.HiddenGroup = 0
        // ORDER BY groups.Name
        // OFFSET @Offset ROWS
        // FETCH NEXT @PageSize ROWS ONLY;

        //public PaginatedList<Folder> GetFolders(int page = 1, int pageSize = 10)
        //{
        //    var dbConnection = _connectionFactory.CreateReadOnlyConnection();
        //    PaginatedList<Folder> groupsList;
        //    const string query =
        //        @"SELECT groups.Name, groups.Description, groups.Slug FROM[group] groups
        //         WHERE groups.HiddenGroup = 0
        //         ORDER BY groups.Name
        //         OFFSET @Offset ROWS
        //         FETCH NEXT @PageSize ROWS ONLY;

        //    SELECT COUNT(*)
        //             FROM[group] groups
        //            WHERE groups.HiddenGroup = 0";

        //        using (var multipleResults = dbConnection.QueryMultipleAsync(query, new
        //        {
        //            Offset = (page - 1) * pageSize,
        //            PageSize = pageSize
        //        }))
        //    {

        //        var results = await multipleResults.ReadAsync<Folder>();
        //        var totalCount = multipleResults.ReadFirst<int>();
        //        groupsList = new PaginatedList<Folder>(results.ToList(), totalCount, page, pageSize);
        //    }

        //    return groupsList;
        //}
    }
}


