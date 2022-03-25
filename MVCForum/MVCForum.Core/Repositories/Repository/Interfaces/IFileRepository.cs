namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using MvcForum.Core.Models.Enums;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the repository for interactions with files.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Method to get all files for a folder.
        /// </summary>
        /// <param name="folderId">Folder to get files for.</param>
        /// <returns>List of <see cref="FileReadViewModel"/>.</returns>
        Task<IEnumerable<FileReadViewModel>> GetFilesAsync(Guid folderId, UploadStatus status = UploadStatus.Verified, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Method to get a file by fileId.
        /// </summary>
        /// <param name="fileId">File to get.</param>
        /// <returns>Requested <see cref="FileReadViewModel"/>.</returns>
        Task<FileReadViewModel> GetFileAsync(Guid fileId, CancellationToken cancellationToken);
        Task<bool> UserHasFileAccessAsync(Guid fileId, Guid userId, CancellationToken cancellationToken);
    }
}
