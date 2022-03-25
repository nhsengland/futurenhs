namespace MvcForum.Core.Repositories.Command.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.FilesAndFolders;

    /// <summary>
    /// Defines the interface for write interactions for files.
    /// </summary>
    public interface IFileCommand
    {
        /// <summary>
        /// Method to add a new <see cref="File"/> to the database.
        /// </summary>
        /// <param name="file">The <see cref="FileWriteViewModel"/> to add.</param>
        /// <returns>The id of the created file.</returns>
        Guid Create(FileWriteViewModel file);

        /// <summary>
        /// Method to update a <see cref="File"/>.
        /// </summary>
        /// <param name="file">Updated file.</param>
        /// <returns>The id of the file.</returns>
        Guid Update(FileWriteViewModel file);

        /// <summary>
        /// Method to delete a <see cref="File"/>
        /// </summary>
        /// <param name="file">The <see cref="File"/> to delete.</param>
        void Delete(FileWriteViewModel file);

        Task<bool> UpdateAsync(FileUpdateViewModel model, CancellationToken cancellationToken = default);
    }
}
