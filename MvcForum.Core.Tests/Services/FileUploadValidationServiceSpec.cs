namespace MvcForum.Core.Tests.Services
{
    using Moq;
    using MvcForum.Core.Interfaces.Helpers;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Services;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;

    [TestFixture]
    public class FileUploadValidationServiceSpec
    {
        private IFileUploadValidationService _fileUploadValidationService;

        private Mock<ILocalizationService> _localizationService;
        private Mock<IValidateFileType> _validateFileType;

        private Mock<HttpPostedFileBase> _postedFile;

        private const string ValidFileName = "test.pdf";
        private const string LongFileName = "A_really_long_file_name_to_break_the_file_name_length_validation_and_test_that_the_validation_is_working.pdf";
        private const string PdfExtension = ".pdf";

        // Validation errors expected
        private const string InvalidFileTypeError = "Invalid file type";
        private const string InvalidContentForExtesionTypeError = "Content does not match extension";
        private const string InvalidFileSizeError = "Invalid file size";
        private const string EmptyFileNameError = "Empty file name";
        private const string FileNameOutOfRangeError = "File name too long";


        [SetUp]
        public void Setup()
        {
            _localizationService = new Mock<ILocalizationService>();
            _validateFileType = new Mock<IValidateFileType>();
            _postedFile = new Mock<HttpPostedFileBase>();

            // Default to valid file name
            _postedFile.Setup(x => x.FileName).Returns(ValidFileName);

            // Default to valid Mime type
            _validateFileType.Setup(x => x.ContentMatchesExtension(It.IsAny<Stream>(), It.IsAny<string>())).Returns(true);

            _fileUploadValidationService = new FileUploadValidationService(_localizationService.Object, _validateFileType.Object);

            _localizationService.Setup(x => x.GetResourceString("File.Error.InvalidType")).Returns(InvalidFileTypeError);
            _localizationService.Setup(x => x.GetResourceString("File.Error.ContentMatchesExtension")).Returns(InvalidContentForExtesionTypeError);
            _localizationService.Setup(x => x.GetResourceString("File.Error.InvalidSize")).Returns(InvalidFileSizeError);
            _localizationService.Setup(x => x.GetResourceString("File.Error.NullName")).Returns(EmptyFileNameError);
            _localizationService.Setup(x => x.GetResourceString("File.Error.NameLength")).Returns(FileNameOutOfRangeError);
        }

        [Test]
        public void ValidateUploadedFile_NullFile_Test()
        {
            Assert.Throws<ArgumentNullException>(delegate { _fileUploadValidationService.ValidateUploadedFile(null); } );
        }

        [Test]
        public void ValidateUploadedFile_ValidationFails_SimpleFileType_Test()
        {
            _postedFile.Setup(x => x.FileName).Returns("test.zzz");

            var result = _fileUploadValidationService.ValidateUploadedFile(_postedFile.Object);

            Assert.True(result.ValidationErrors.Contains(InvalidFileTypeError));
            Assert.IsNull(result.MimeType);
        }

        [Test]
        public void ValidateUploadedFile_ValidationFails_ComplexFileType_Test()
        {
            _validateFileType.Setup(x => x.ContentMatchesExtension(It.IsAny<Stream>(), It.IsAny<string>())).Returns(false);

            var result = _fileUploadValidationService.ValidateUploadedFile(_postedFile.Object);

            Assert.True(result.ValidationErrors.Contains(InvalidContentForExtesionTypeError));
            Assert.IsNull(result.MimeType);
        }

        [Test]
        public void ValidateUploadedFile_ValidationFails_FileSizeOutOfRange_Test()
        {
            _postedFile.Setup(x => x.ContentLength).Returns(250000001);

            var result = _fileUploadValidationService.ValidateUploadedFile(_postedFile.Object);

            Assert.True(result.ValidationErrors.Contains(InvalidFileSizeError));
            Assert.AreEqual(MimeMapping.GetMimeMapping(PdfExtension), result.MimeType);
        }

        [Test]
        public void ValidateUploadedFile_ValidationFails_FileNameEmpty_Test()
        {
            // Use extension only to trigger this validation fail
            _postedFile.Setup(x => x.FileName).Returns(PdfExtension);

            var result = _fileUploadValidationService.ValidateUploadedFile(_postedFile.Object);

            Assert.True(result.ValidationErrors.Contains(EmptyFileNameError));
            Assert.AreEqual(MimeMapping.GetMimeMapping(PdfExtension), result.MimeType);
        }

        [Test]
        public void ValidateUploadedFile_ValidationFails_FileNameOutOfRange_Test()
        {
            _postedFile.Setup(x => x.FileName).Returns(LongFileName);

            var result = _fileUploadValidationService.ValidateUploadedFile(_postedFile.Object);

            Assert.True(result.ValidationErrors.Contains(FileNameOutOfRangeError));
            Assert.AreEqual(MimeMapping.GetMimeMapping(PdfExtension), result.MimeType);
        }

        [TestCase("test.doc")]
        [TestCase("test.docx")]
        [TestCase("test.xls")]
        [TestCase("test.xlsx")]
        [TestCase("test.pdf")]
        [TestCase("test.ppt")]
        [TestCase("test.pptx")]
        public void ValidateUploadedFile_ValidationPasses_Test(string fileName)
        {
            // Mime type for checking result value
            string mimeType = MimeMapping.GetMimeMapping(Path.GetExtension(fileName));

            _postedFile.Setup(x => x.FileName).Returns(fileName);

            var result = _fileUploadValidationService.ValidateUploadedFile(_postedFile.Object);

            Assert.True(result.ValidationErrors.ToList().Count == 0);
            Assert.AreEqual(mimeType, result.MimeType);
        }
    }
}
