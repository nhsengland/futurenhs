//-----------------------------------------------------------------------
// <copyright file="IGroupRepository.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Folder = MvcForum.Core.Repositories.Models.Folder;

    public interface IFolderRepository
    {
        /// <summary>
        /// Create a folder.
        /// </summary>
        /// <param name="folder">Folder to create.</param>
        /// <returns></returns>
        Task<Folder> Create(Folder folder);

        /// <summary>
        /// Update a folder. This is also used to delete (change status).
        /// </summary>
        /// <param name="folder">Folder to update.</param>
        /// <returns></returns>
        Task<Folder> Update(Folder folder);

        /// <summary>
        /// Gets folder based on Id.
        /// </summary>
        /// <param name="folderId">Id to get folder.</param>
        /// <returns></returns>
        Task<Folder> GetFolder(Guid folderId);

        /// <summary>
        /// Gets folders based on parent Id. If parent Id is null then they are rot folders.
        /// </summary>
        /// <param name="folderId">parent Id to get folders.</param>
        /// <returns></returns>
        Task<List<Folder>> GetChildFolders(Guid folderId);


    }
}
