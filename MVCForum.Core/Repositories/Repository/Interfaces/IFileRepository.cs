//-----------------------------------------------------------------------
// <copyright file="FileRepository.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the repository for interactions with files.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Method to get all files for a folder.
        /// </summary>
        /// <param name="folderId">Folder to get files for.</param>
        /// <returns>List of <see cref="File"/>.</returns>
        List<File> GetFiles(Guid folderId);

        /// <summary>
        /// Method to get a file by fileId.
        /// </summary>
        /// <param name="fileId">File to get.</param>
        /// <returns>Requested <see cref="File"/>.</returns>
        File GetFile(Guid fileId);
    }
}
