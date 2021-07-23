//-----------------------------------------------------------------------
// <copyright file="FolderRepository.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Groups.Repository.Database
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    //using DbFolder = MvcForum.Core.Repositories.Database.Models.Folder;
    using Folder = MvcForum.Core.Repositories.Models.Folder;

    public class FolderRepository : IFolderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FolderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Create a folder.
        /// </summary>
        /// <param name="folder">Folder to create.</param>
        /// <returns></returns>
        public async Task<Folder> Create(Folder folder)
        {
            var dbConnection = _connectionFactory.CreateWriteConnection();

            //Create the folder

            return null;
        }

        /// <summary>
        /// Update a folder. This is also used to delete (change status).
        /// </summary>
        /// <param name="folder">Folder to update.</param>
        /// <returns></returns>
        public async Task<Folder> Update(Folder folder)
        {
            var dbConnection = _connectionFactory.CreateWriteConnection();

            //Update the folder

            return null;
        }

        /// <summary>
        /// Gets folders based on parent Id. If parent Id is null then they are rot folders.
        /// </summary>
        /// <param name="parentId">Nullable parent Id to get folders.</param>
        /// <returns></returns>
        public async Task<List<Folder>> GetFolders(Guid? parentId = null)
        {
            //var dbConnection = _connectionFactory.CreateReadOnlyConnection();

            // Hard coded for now, need to hook up to DB and use Dapper to convert.

            List<Folder> folderList = new List<Folder>();

            if (parentId.HasValue)
            {
                folderList.AddRange(new List<Folder>
                {
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Child folder 1", Description = "Child folder 1 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 },
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Child folder 2", Description = "Child folder 2 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 },
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Child folder 3", Description = "Child folder 3 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 },
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Child folder 4", Description = "Child folder 4 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 }
                });
            }
            else
            {
                folderList.AddRange(new List<Folder>
                {
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Root folder 1", Description = "Root folder 1 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 },
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Root folder 2", Description = "Root folder 2 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 },
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Root folder 3", Description = "Root folder 3 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 },
                    new Folder() { FolderId = Guid.NewGuid(), FolderName = "Root folder 4", Description = "Root folder 4 description", DateAdded = DateTime.Now, AddedBy = Guid.NewGuid(), ParentId = parentId, Status = 1 }
                });
            }

            return folderList;
        }

        /*
        public async Task<PaginatedList<Folder>> GetFolders(int page = 1, int pageSize = 10)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            PaginatedList<Folder> groupsList;
            const string query =
                @"SELECT groups.Name, groups.Description, groups.Slug FROM [group] groups
                 WHERE groups.HiddenGroup = 0
                 ORDER BY groups.Name
                 OFFSET @Offset ROWS
                 FETCH NEXT @PageSize ROWS ONLY;

                 SELECT COUNT(*)
                 FROM [group] groups 
                 WHERE groups.HiddenGroup = 0";

            using (var multipleResults = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            }))
            {

                var results = await multipleResults.ReadAsync<Folder>();
                var totalCount = multipleResults.ReadFirst<int>();
                groupsList = new PaginatedList<Folder>(results.ToList(), totalCount, page, pageSize);
            }

            return groupsList;
        }*/
    }
}
