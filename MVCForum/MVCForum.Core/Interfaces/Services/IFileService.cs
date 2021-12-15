namespace MvcForum.Core.Interfaces.Services
{
    using Azure.Storage.Sas;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using FileStatus = Models.Enums.UploadStatus;

    /// <summary>
    /// Defines methods for interacting with files.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Method Create a new File in the db.
        /// </summary>
        /// <param name="file">The File to create.</param>
        /// <returns>The id of the file.</returns>
        Guid Create(FileWriteViewModel file);

        /// <summary>
        /// Method to update an existing File in the db.
        /// </summary>
        /// <param name="file">The updated file.</param>
        /// <returns>The id of the file.</returns>
        Guid Update(FileWriteViewModel file);

        /// <summary>
        /// Method to update an existing File in the db.
        /// </summary>
        /// <param name="file">The updated file.</param>
        /// <returns>Bool success/fail.</returns>
        Task<bool> UpdateAsync(FileUpdateViewModel file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method to get a File by id.
        /// </summary>
        /// <param name="id">The id of the File.</param>
        /// <returns>The requested File.</returns>
        Task<FileReadViewModel> GetFileAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Method to get all files for folder.
        /// </summary>
        /// <param name="folderId">The id of the folder.</param>
        /// <returns>List of files <see cref="List{File}"/></returns>
        Task<IEnumerable<FileReadViewModel>> GetFilesAsync(Guid folderId, FileStatus status = FileStatus.Verified, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Method to perform soft delete of a <see cref="File"/>.
        /// </summary>
        /// <param name="id">The id of the File.</param>
        void Delete(FileWriteViewModel id);

        /// <summary>
        /// Upload a file to blob storage.
        /// </summary>
        /// <param name="file">Posted file to upload.</param>
        /// <returns></returns>
        Task<UploadBlobResult> UploadFileAsync(HttpPostedFileBase file, string contentType, CancellationToken cancellationToken);

        /// <summary>
        /// Perform file validation before saving to DB and performing file upload.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        ValidateBlobResult FileValidation(HttpPostedFileBase file);

        /// <summary>
        /// Generate a Url to a blob to redirect to with a user delegation sas token.
        /// </summary>
        /// <param name="blobName">Name of blob to redirect to (blob storage name rather than original file name).</param>
        /// <param name="downloadPermissions">Permissions to be applied to the SasBuilder.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetRelativeDownloadUrlAsync(string blobName, BlobSasPermissions downloadPermissions, CancellationToken cancellationToken);
        Task<bool> UserHasFileAccessAsync(Guid fileId, Guid userId, CancellationToken cancellationToken);
    }
}
