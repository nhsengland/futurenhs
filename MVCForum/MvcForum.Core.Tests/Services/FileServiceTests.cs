namespace MvcForum.Core.Tests.Services
{
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using MvcForum.Core.Interfaces.Providers;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.FilesAndFolders;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using MvcForum.Core.Services;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using FileStatus = Models.Enums.UploadStatus;

    [TestFixture]
    public class FileServiceTests
    {
        private const string ValidationError = "Error";
        private const string ValidationMimeType = "MimeType";

        private const string BlobName = "blobName.pdf";

        private const string InvalidFileUploadConnectionString = "Not valid";
        private const string InvalidFileUploadContainerName = "Not valid";

        private const string FileParameterName = "file";
        private const string FileIdParameterName = "id";
        private const string BlobNameParameterName = "blobName";
        private const string FileDownloadEndpointName = "FileDownloadEndpoint";
        private const string FolderIdParameterName = "folderId";
        private const string FileUploadConnectionStringName = "FileUploadConnectionString";
        private const string FileContainerNameName = "FileContainerName";

        private const string FileUploadInvalidConnectionStringFormat = "Settings must be of the form \"name=value\".";

        private IFileService _fileService;

        private Mock<IFileCommand> _fileCommand;

        private Mock<IFileRepository> _fileRepository;

        private Mock<IFileUploadValidationService> _fileUploadValidationService;

        private Mock<IConfigurationProvider> _configurationProvider;

        private Mock<IMemoryCache> _memoryCache;

        private Mock<FileWriteViewModel> _fileWriteViewModel;

        private readonly Guid _fileId = Guid.NewGuid();
        private readonly Guid _folderd = Guid.NewGuid();

        private FileReadViewModel _fileReadViewModel;
        private ValidateBlobResult _validateBlobResult;

        private Mock<HttpPostedFileBase> _postedFile;

        [SetUp]
        public void Setup()
        {
            _fileCommand = new Mock<IFileCommand>();
            _fileRepository = new Mock<IFileRepository>();

            _fileUploadValidationService = new Mock<IFileUploadValidationService>();
            _configurationProvider = new Mock<IConfigurationProvider>();
            _memoryCache = new Mock<IMemoryCache>();

            _postedFile = new Mock<HttpPostedFileBase>();

            _fileService = new FileService(_fileCommand.Object, _fileRepository.Object,
                                            _fileUploadValidationService.Object, _configurationProvider.Object, _memoryCache.Object);

            _fileWriteViewModel = new Mock<FileWriteViewModel>();

            _fileCommand.Setup(x => x.Create(_fileWriteViewModel.Object)).Returns(_fileId);
            _fileCommand.Setup(x => x.Update(_fileWriteViewModel.Object)).Returns(_fileId);

            _fileReadViewModel = new FileReadViewModel() { Id = _fileId };

            _validateBlobResult = new ValidateBlobResult();

            _fileUploadValidationService.Setup(x => x.ValidateUploadedFile(It.IsAny<HttpPostedFileBase>())).Returns(_validateBlobResult);
        }

        [Test]
        public void Create_FileParameterNull_ThrowsException()
        {
            var response = Assert.Throws<ArgumentNullException>(delegate { _fileService.Create(null); });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileParameterName, response.ParamName);
        }

        [Test]
        public void Create_Success_ReturnsGuid()
        {
            var result = _fileService.Create(_fileWriteViewModel.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Guid>(result);
            Assert.AreEqual(_fileId, result);
        }

        [Test]
        public void Update_FileParameterNull_ThrowsException()
        {
            var response = Assert.Throws<ArgumentNullException>(delegate { _fileService.Update(null); });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileParameterName, response.ParamName);
        }

        [Test]
        public void Update_Success_ReturnsGuid()
        {
            var result = _fileService.Update(_fileWriteViewModel.Object);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Guid>(result);
            Assert.AreEqual(_fileId, result);
        }

        [Test]
        public void Delete_FileParameterNull_ThrowsException()
        {
            var response = Assert.Throws<ArgumentNullException>(delegate { _fileService.Delete(null); });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileParameterName, response.ParamName);
        }

        [Test]
        public void Delete_Success_Test()
        {
            _fileCommand.Setup(f => f.Delete(It.IsAny<FileWriteViewModel>()));

            _fileService.Delete(_fileWriteViewModel.Object);

            _fileCommand.Verify(x => x.Delete(_fileWriteViewModel.Object));
        }

        [Test]
        public void GetFileAsync_FileIdEmpty_ThrowsException()
        {
            var response = Assert.Throws<ArgumentOutOfRangeException>(delegate { _fileService.GetFileAsync(Guid.Empty, CancellationToken.None); });

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FileIdParameterName, response.ParamName);
        }

        [Test]
        public void GetFileAsync_NoFileFound_ReturnsNull()
        {
            _fileRepository.Setup(x => x.GetFileAsync(It.IsAny<Guid>(), CancellationToken.None)).Returns(Task.FromResult<FileReadViewModel>(null));

            var result = _fileService.GetFileAsync(_fileId, CancellationToken.None).Result;

            Assert.IsNull(result);
        }

        [Test]
        public void GetFileAsync_Success_ReturnsFileReadViewModel()
        {
            _fileRepository.Setup(x => x.GetFileAsync(It.IsAny<Guid>(), CancellationToken.None)).Returns(Task.FromResult(_fileReadViewModel));

            var result = _fileService.GetFileAsync(_fileId, CancellationToken.None).Result;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<FileReadViewModel>(result);
            Assert.AreEqual(_fileId, result.Id);
        }

        [Test]
        public void GetRelativeDownloadUrlAsync_BlobNameParameterNull_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await _fileService.GetRelativeDownloadUrlAsync(null, Azure.Storage.Sas.BlobSasPermissions.Read, CancellationToken.None);
            });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(BlobNameParameterName, response.ParamName);
        }

        [Test]
        public void GetRelativeDownloadUrlAsync_FileDownloadEndpointNull_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await _fileService.GetRelativeDownloadUrlAsync(BlobName, Azure.Storage.Sas.BlobSasPermissions.Read, CancellationToken.None);
            });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileDownloadEndpointName, response.ParamName);
        }

        [Test]
        public void GetFilesAsync_FolderIdParameterNull_ThrowsException()
        {
            var response = Assert.Throws<ArgumentOutOfRangeException>(delegate { _fileService.GetFilesAsync(Guid.Empty, FileStatus.Verified, CancellationToken.None); });

            Assert.IsInstanceOf<ArgumentOutOfRangeException>(response);
            Assert.AreEqual(FolderIdParameterName, response.ParamName);
        }

        [Test]
        public void GetFilesAsync_NoFilesFoundForFolderId_ReturnsNull()
        {
            _fileRepository.Setup(x => x.GetFilesAsync(It.IsAny<Guid>(), FileStatus.Verified, CancellationToken.None)).Returns(Task.FromResult<IEnumerable<FileReadViewModel>>(null));

            var result = _fileService.GetFilesAsync(_folderd, FileStatus.Verified, CancellationToken.None).Result;

            Assert.IsNull(result);
        }

        [Test]
        public void GetFilesAsync_Success_ReturnsFileReadViewModelEnumerable()
        {
            var files = new List<FileReadViewModel>() { _fileReadViewModel };

            _fileRepository.Setup(x => x.GetFilesAsync(It.IsAny<Guid>(), FileStatus.Verified, CancellationToken.None)).Returns(Task.FromResult<IEnumerable<FileReadViewModel>>(files));

            var result = _fileService.GetFilesAsync(_folderd, FileStatus.Verified, CancellationToken.None).Result;

            var expectedFileCount = files.Count;
            var fileCount = result.ToList().Count;
            var fileIdReturned = result.FirstOrDefault().Id;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<FileReadViewModel>>(result);
            Assert.AreEqual(expectedFileCount, fileCount);
            Assert.AreEqual(_fileId, fileIdReturned);
        }

        // Upload file

        [Test]
        public void UploadFileAsync_FileParameterNull_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => { 
                await _fileService.UploadFileAsync(null, string.Empty, CancellationToken.None); 
            });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileParameterName, response.ParamName);
        }

        [Test]
        public void UploadFileAsync_FileUploadConnectionStringNotSet_ThrowsException()
        {
            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await _fileService.UploadFileAsync(_postedFile.Object, string.Empty, CancellationToken.None);
            });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileUploadConnectionStringName, response.ParamName);
        }

        [Test]
        public void UploadFileAsync_FileContainerNameNotSet_ThrowsException()
        {
            _configurationProvider.Setup(x => x.FileUploadConnectionString).Returns(InvalidFileUploadConnectionString);

            var response = Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await _fileService.UploadFileAsync(_postedFile.Object, string.Empty, CancellationToken.None);
            });

            Assert.IsInstanceOf<ArgumentNullException>(response);
            Assert.AreEqual(FileContainerNameName, response.ParamName);
        }

        [Test]
        public void UploadFileAsync_FileUploadConnectionStringHasInvalidFormat_ThrowsException()
        {
            _configurationProvider.Setup(x => x.FileUploadConnectionString).Returns(InvalidFileUploadConnectionString);
            _configurationProvider.Setup(x => x.FileContainerName).Returns(InvalidFileUploadContainerName);

            var response = Assert.ThrowsAsync<FormatException>(async () => {
                await _fileService.UploadFileAsync(_postedFile.Object, string.Empty, CancellationToken.None);
            });

            Assert.IsInstanceOf<FormatException>(response);
            Assert.AreEqual(FileUploadInvalidConnectionStringFormat, response.Message);
        }

        [Test]
        public void FileValidation_NotValid_ReturnsSingleValidationError()
        {
            _validateBlobResult.ValidationErrors = new List<string>() { ValidationError };

            var result = _fileService.FileValidation(It.IsAny<HttpPostedFileBase>());

            var expectedValidationErrorCount = _validateBlobResult.ValidationErrors.Count();
            var actualValidationErrorCount = result.ValidationErrors.Count();
            var validationErrorReturned = result.ValidationErrors.FirstOrDefault();

            Assert.AreEqual(expectedValidationErrorCount, actualValidationErrorCount);
            Assert.AreEqual(ValidationError, validationErrorReturned);
        }

        [Test]
        public void FileValidation_Valid_ReturnsValididateBlobResultWithNoErrorsAndMimeTypeSet()
        {
            _validateBlobResult.MimeType = ValidationMimeType;

            var result = _fileService.FileValidation(It.IsAny<HttpPostedFileBase>());

            var actualValidationErrorCount = result.ValidationErrors.Count();

            Assert.AreEqual(0, actualValidationErrorCount);
            Assert.AreEqual(ValidationMimeType, result.MimeType);
        }
    }
}
