namespace MvcForum.Core.Interfaces.Services
{
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines methods for interacting with files.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Method Create a new <see cref="FileReadViewModel"/> in the db.
        /// </summary>
        /// <param name="file">The <see cref="CreateGroupFileViewModel"/> to create.</param>
        /// <returns>The id of the file.</returns>
        Guid Create(CreateGroupFileViewModel file);

        /// <summary>
        /// Method to get a <see cref="FileReadViewModel"/> by id.
        /// </summary>
        /// <param name="id">The id of the <see cref="FileReadViewModel"/>.</param>
        /// <returns>The requested <see cref="FileReadViewModel"/>.</returns>
        FileReadViewModel GetFile(Guid id);

        /// <summary>
        /// Method to get all files for folder.
        /// </summary>
        /// <param name="folderId">The id of the folder.</param>
        /// <returns>List of files <see cref="List{File}"/></returns>
        IEnumerable<FileReadViewModel> GetFiles(Guid folderId);

        /// <summary>
        /// Method to perform soft delete of a <see cref="File"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="FileReadViewModel"/>.</param>
        void Delete(Guid id);
    }
}
