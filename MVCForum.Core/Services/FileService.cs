namespace MvcForum.Core.Services
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines methods for read and write operations on files.
    /// </summary>
    public class FileService : IFileService
    {
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
        /// Constructs a new instance of the <see cref="FileService"/>.
        /// </summary>
        /// <param name="fileCommand">Instance of <see cref="IFileCommand"/>.</param>
        /// <param name="fileRepository">Instance of <see cref="IFileRepository"/>.</param>
        public FileService(IFileCommand fileCommand, IFileRepository fileRepository, IMembershipService membershipService)
        {
            _fileCommand = fileCommand;
            _fileRepository = fileRepository;
            LoggedOnReadOnlyUser = membershipService.GetUser(System.Web.HttpContext.Current.User.Identity.Name, true);
        }

        /// <summary>
        /// Method to create a new <see cref="FileReadViewModel"/> in the database.
        /// </summary>
        /// <param name="file">The file to create.</param>
        /// <returns>The file id.</returns>
        public Guid Create(CreateGroupFileViewModel file)
        {

            var fileCreate = new File
            {
                FileName = file.Name,
                Title = file.Name,
                Description = file.Description,
                CreatedDate = DateTime.Now,
                ParentFolder = file.FolderId,
                CreatedBy = LoggedOnReadOnlyUser.Id
            };

            return _fileCommand.Create(fileCreate);
        }

        /// <summary>
        /// Method set the soft delete flag on a file.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to get a file by id.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <returns>The requested <see cref="FileReadViewModel"/>.</returns>
        public FileReadViewModel GetFile(Guid id)
        {
            return _fileRepository.GetFile(id);
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
    }
}
