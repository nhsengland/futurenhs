//-----------------------------------------------------------------------
// <copyright file="FolderRepository.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MvcForum.Core.Models.General;
using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
using MvcForum.Core.Repositories.Models;
using MvcForum.Core.Repositories.Repository.Interfaces;

namespace MvcForum.Core.Repositories.Repository
{
    //using DbFolder = MvcForum.Core.Repositories.Database.Models.Folder;

    public class FolderRepository : IFolderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FolderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public PaginatedList<FolderReadViewModel> GetRootFoldersForGroup(string groupSlug, int page = 1, int pageSize = 10)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            PaginatedList<FolderReadViewModel> folders;
            const string query =
                @"SELECT folders.Id AS FolderId, folders.Name AS FolderName, folders.FileCount 
                FROM Folder folders
                JOIN [Group] groups ON groups.Id = folders.ParentGroup
                WHERE groups.Slug = @GroupSlug 
                AND folders.ParentFolder IS NULL AND folders.IsDeleted = 0
                ORDER BY folders.Name
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

        public PaginatedList<FolderReadViewModel> GetChildFoldersForFolder(Guid parentFolderId, int page = 1, int pageSize = 10)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            PaginatedList<FolderReadViewModel> folders;
            const string query =
                @"SELECT Id AS FolderId, Name AS FolderName, FileCount 
                FROM Folder
                WHERE ParentFolder = @ParentFolder 
                AND IsDeleted = 0
                ORDER BY Name
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
                WHERE folders.Id = @FolderId;";

            var result = dbConnection.QueryFirstOrDefault<FolderReadViewModel>(query, new {FolderId = folderId});
            return result;
        }

        public bool UserIsAdmin(string groupSlug, Guid userId)
        {
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

                return membershipRole?.ToLower() == "admin" || groupRole?.ToLower() == "admin";
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


