namespace MvcForum.Core.Interfaces.Services
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Sas;
    using Microsoft.WindowsAzure.Storage.Blob;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;

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
        /// Method to get a File by id.
        /// </summary>
        /// <param name="id">The id of the File.</param>
        /// <returns>The requested File.</returns>
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
        /// <param name="id">The id of the File.</param>
        void Delete(FileWriteViewModel id);

        /// <summary>
        /// Upload a file to blob storage.
        /// </summary>
        /// <param name="file">Posted file to upload.</param>
        /// <returns></returns>
        Task<UploadBlobResult> UploadFileAsync(HttpPostedFileBase file);

        /// <summary>
        /// Perform simple file validation. Use this before saving to DB and performing file upload.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        UploadBlobResult SimpleFileValidation(HttpPostedFileBase file);

        /// <summary>
        /// Generate a Url to a blob to redirect to with a user delegation sas token.
        /// </summary>
        /// <param name="blobName">Name of blob to redirect to (blob storage name rather than original file name).</param>
        /// <param name="downloadPermissions">Permissions to be applied to the SasBuilder.</param>
        /// <returns></returns>
        Task<string> GetRelativeDownloadUrlAsync(string blobName, BlobSasPermissions downloadPermissions);
    }
}
