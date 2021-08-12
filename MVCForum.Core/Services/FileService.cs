using MvcForum.Core.Interfaces.Providers;

namespace MvcForum.Core.Services
{
    using Azure.Identity;
    using Azure.Storage.Blobs;
    using Azure.Storage.Sas;
    using Microsoft.WindowsAzure.Storage;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Defines methods for read and write operations on files.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IConfigurationProvider _configurationProvider;

        /// <summary>
        /// Instance of the <see cref="IFileCommand"/> for file write operations.
        /// </summary>
        private readonly IFileCommand _fileCommand;

        /// <summary>
        /// Instance of the <see cref="IFileRepository"/> for file read operations.
        /// </summary>
        private readonly IFileRepository _fileRepository;

        private readonly MembershipUser _loggedOnReadOnlyUser;

        /// <summary>
        /// Instance of the <see cref="IFileRepoIFileUploadValidationServicesitory"/> for file validation operations.
        /// </summary>
        private readonly IFileUploadValidationService _fileUploadValidationService;

        /// <summary>
        /// Constructs a new instance of the <see cref="FileService"/>.
        /// </summary>
        /// <param name="fileCommand">Instance of <see cref="IFileCommand"/>.</param>
        /// <param name="fileRepository">Instance of <see cref="IFileRepository"/>.</param>
        public FileService(IFileCommand fileCommand, IFileRepository fileRepository, IMembershipService membershipService, IFileUploadValidationService fileUploadValidationService, IConfigurationProvider configurationProvider)
        {
            _fileCommand = fileCommand;
            _fileRepository = fileRepository;
            _loggedOnReadOnlyUser = membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);
            _fileUploadValidationService = fileUploadValidationService;
            _configurationProvider = configurationProvider;
        }

        /// <summary>
        /// Method to create a new <see cref="FileReadViewModel"/> in the database.
        /// </summary>
        /// <param name="file">The file to create.</param>
        /// <returns>The file id.</returns>
        public Guid Create(FileWriteViewModel file)
        {
            return _fileCommand.Create(file);
        }

        // <summary>
        /// Method to create a new <see cref="FileReadViewModel"/> in the database.
        /// </summary>
        /// <param name="file">The file to create.</param>
        /// <returns>The file id.</returns>
        public Guid Update(FileWriteViewModel file)
        {
            return _fileCommand.Update(file);
        }

        /// <summary>
        /// Method set the soft delete flag on a file.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        public void Delete(FileWriteViewModel file)
        {
            _fileCommand.Delete(file);
        }

        /// <summary>
        /// Method to get a file by id.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <returns>The requested <see cref="FileReadViewModel"/>.</returns>
        public FileReadViewModel GetFile(Guid id)
        {
            var file = _fileRepository.GetFile(id);

            if (string.IsNullOrWhiteSpace(file.ModifiedUserName) || string.IsNullOrWhiteSpace(file.ModifiedUserSlug))
            {
                file.ModifiedUserName = file.UserName;
                file.ModifiedUserSlug = file.UserSlug;
            }

            if (!file.ModifiedDate.HasValue)
            {
                file.ModifiedDate = file.CreatedDate;
            }

            return file;
        }

        /// <summary>
        /// Generate a Url to a blob to redirect to with a user delegation sas token.
        /// </summary>
        /// <param name="blobName">Name of blob to redirect to (blob storage name rather than original file name).</param>
        /// <param name="downloadPermissions">Permissions to be applied to the SasBuilder.</param>
        /// <returns></returns>
        public async Task<string> GetDownloadUrlAsync(string blobName, BlobSasPermissions downloadPermissions)
        {
            // Set the blob service client
            BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(_configurationProvider.GetFileDownloadEndpoint()), new DefaultAzureCredential());

            // Generate the user delegation key
            var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(40));

            if (userDelegationKey != null)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = _configurationProvider.GetFileContainerName(),
                    BlobName = blobName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(40)
                };

                // Specify read and write permissions for the SAS. Pass these in?
                sasBuilder.SetPermissions(downloadPermissions);

                // Add the SAS token to the blob URI.
                BlobUriBuilder blobUriBuilder = new BlobUriBuilder(blobServiceClient.Uri)
                {
                    // Set container and blob name
                    BlobContainerName = _configurationProvider.GetFileContainerName(),
                    BlobName = blobName,
                    // Specify the user delegation key.
                    Sas = sasBuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName)
                };

                // Build the blob redirect path required
                return $"{blobUriBuilder.BlobContainerName}/{blobUriBuilder.BlobName}?{blobUriBuilder.Sas}";
            }
            throw new InvalidOperationException("Unable to generate download token");
        }

        /// <summary>
        /// Method to get a list of files for folder.
        /// </summary>
        /// <param name="folderId">The folder id to get files for.</param>
        /// <returns>List of file <see cref="List{File}"/></returns>
        public IEnumerable<FileReadViewModel> GetFiles(Guid folderId)
        {
            return _fileRepository.GetFiles(folderId);
        }

        /// <summary>
        /// Upload a file to blob storage.
        /// </summary>
        /// <param name="file">Posted file to upload.</param>
        /// <returns></returns>
        public async Task<UploadBlobResult> UploadFileAsync(HttpPostedFileBase file)
        {
            // TODO - implement validation including of type (MIME detective), add this to _fileUploadValidationService
            var result = _fileUploadValidationService.ValidateUploadedFile(file, false);

            if (!result.ValidationErrors.Any())
            {
                var storageAccount = CloudStorageAccount.Parse(_configurationProvider.GetFileUploadConnectionString());

                var blobStorage = storageAccount.CreateCloudBlobClient();
                var container = blobStorage.GetContainerReference(_configurationProvider.GetFileContainerName());

                var uniqueBlobName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";

                result.UploadedFileName = uniqueBlobName;

                var blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = file.ContentType;
                blob.Properties.ContentDisposition = $"attachment; filename={file.FileName}";
                await blob.UploadFromStreamAsync(file.InputStream);

                // set the attributes required to be saved in the DB?
                // Question, are the values from Posted file 'safe' enough or should we get from uploaded blob?
                result.UploadedFileHash = Convert.FromBase64String(blob.Properties.ContentMD5);
                result.UploadSuccessful = true;
            }
            return result;
        }

        /// <summary>
        /// Perform simple file validation. Use this before saving to DB and performing file upload.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public UploadBlobResult SimpleFileValidation(HttpPostedFileBase file)
        {
            return _fileUploadValidationService.ValidateUploadedFile(file, true);
        }
    }
}
