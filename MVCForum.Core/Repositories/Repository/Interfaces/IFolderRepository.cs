//-----------------------------------------------------------------------
// <copyright file="IFolderRepository.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Models;

    public interface IFolderRepository
    {
        /// <summary>
        /// Gets folder based on Id.
        /// </summary>
        /// <param name="folderId">Id to get folder.</param>
        /// <returns>Folder Meta data for the Id provided</returns>
        FolderReadViewModel GetFolder(Guid folderId);

        /// <summary>
        /// Get folder by Id, folder name and parent - validate folder exists for create/update, i.e. no duplicate names allowed.
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="folderName"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        FolderReadViewModel GetFolder(Guid? folderId, string folderName, Guid? parentFolder);

        /// <summary>
        /// Gets root folder for a group.
        /// </summary>
        /// <param name="groupSlug">Group slug to find root folders for</param>
        /// <param name="page">page number to get results for</param>
        /// <param name="pageSize">number of results per page</param>
        /// <returns>A paginated list of folder meta data</returns>
        PaginatedList<FolderReadViewModel> GetRootFoldersForGroup(string groupSlug, int page = 1, int pageSize = 999);

        /// <summary>
        /// Gets folders based on parent Id
        /// </summary>
        /// <param name="parentFolderId">parent Id to get folders.</param>
        /// <param name="page">page number to get results for</param>
        /// <param name="pageSize">number of results per page</param>
        /// <returns>A paginated list of folder meta data</returns>
        PaginatedList<FolderReadViewModel> GetChildFoldersForFolder(Guid parentFolderId, int page = 1, int pageSize = 999);

        /// <summary>
        /// Returns a boolean indicating if the user is either a Site Admin or Group Admin
        /// </summary>
        /// <param name="groupSlug">Group slug to identify the group</param>
        /// <param name="userId">User id to check the role for(they may</param>
        /// <returns></returns>
        bool UserIsAdmin(string groupSlug, Guid userId);

        /// <summary>
        /// Returns Breadcrumb trail to allow the user to navigate back up the tree
        /// </summary>
        /// <param name="folderId">Id of the bottom element in the tree to work back from</param>
        /// <returns></returns>
        IEnumerable<BreadCrumbItem> GenerateBreadcrumbTrail(Guid folderId);
    }
}
