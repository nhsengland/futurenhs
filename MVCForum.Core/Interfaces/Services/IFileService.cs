namespace MvcForum.Core.Interfaces.Services
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines methods for interacting with files.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Method Create a new <see cref="File"/> in the db.
        /// </summary>
        /// <param name="file">The <see cref="File"/> to create.</param>
        /// <returns>The id of the file.</returns>
        Guid Create(Repositories.Database.Models.File file);

        /// <summary>
        /// Method to get a <see cref="File"/> by id.
        /// </summary>
        /// <param name="id">The id of the <see cref="File"/>.</param>
        /// <returns>The requested <see cref="File"/>.</returns>
        File GetFile(Guid id);

        /// <summary>
        /// Method to get all files for folder.
        /// </summary>
        /// <param name="folderId">The id of the folder.</param>
        /// <returns>List of files <see cref="List{File}"/></returns>
        IEnumerable<File> GetFiles(Guid folderId);

        /// <summary>
        /// Method to perform soft delete of a <see cref="FIle"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="File"/>.</param>
        void Delete(Guid id);
    }
}
