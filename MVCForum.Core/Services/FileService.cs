using MvcForum.Core.Interfaces.Providers;

namespace MvcForum.Core.Services
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
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
        private IFileCommand _fileCommand { get; set; }

        /// <summary>
        /// Instance of the <see cref="IFileRepository"/> for file read operations.
        /// </summary>
        private IFileRepository _fileRepository { get; set; }

        private MembershipUser LoggedOnReadOnlyUser;

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
            LoggedOnReadOnlyUser = membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);
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

            if (file.FileUrl != null)
            {
                file.FileUrl = $"{_configurationProvider.GetFileDownloadEndpoint()}/{file.FileUrl}";
            }

            return file;
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
            var result = new UploadBlobResult();
            try
            {
                // TODO - implement validation including of type (MIME detective), possibly add FileValidationService?
                result = _fileUploadValidationService.ValidateUploadedFile(file, false);

                if (!result.ValidationErrors.Any())
                {
 
                    var storageAccount = CloudStorageAccount.Parse(_configurationProvider.GetFileUploadConnectionString());

                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    var container = blobStorage.GetContainerReference(_configurationProvider.GetFileContainerName());
                    
                    var uniqueBlobName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";

                    result.UploadedFileName = uniqueBlobName;

                    var blob = container.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = file.ContentType;
                    await blob.UploadFromStreamAsync(file.InputStream);

                    // set the attributes required to be saved in the DB?
                    // Question, are the values from Posted file 'safe' enough or should we get from uploaded blob?
                    result.UploadedFileHash = Convert.FromBase64String(blob.Properties.ContentMD5);
                    result.UploadSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                // TODO Deal with any unhandled exception...
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
            var result = new UploadBlobResult();
            try
            {
                result =  _fileUploadValidationService.ValidateUploadedFile(file, true);
            }
            catch (Exception ex)
            {
                // TODO Deal with any exception...
            }
            return result;
        }
    }
}
