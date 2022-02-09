namespace MvcForum.Core.Tests.Utilities
{
    using MvcForum.Core.Interfaces.Helpers;
    using MvcForum.Core.Utilities;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Text;

    [TestFixture]
    public class FileTypeValidatorTests
    {
        private IValidateFileType _fileTypeValidator;

        // Valid extensions as testing with .docx file
        private const string ValidDocxExtensionWithDot = ".docx";
        private const string ValidDocxExtensionWithoutDot = "docx";

        // Invalid extensions as testing with .docx file
        private const string InvalidExtensionWithDot = ".pdf";
        private const string InvalidExtensionWithoutDot = "pdf";

        private const string FileStreamParameterName = "fileStream";
        private const string FileExtensionParameterName = "fileExtension";

        // Testing with a docx file
        private const string TestFilePath = @"\TestResources\Files\ValidFileType.docx";

        private Stream _fileStream = new MemoryStream(Encoding.UTF8.GetBytes("This is the text."));

        [SetUp]
        public void Setup()
        {
            _fileTypeValidator = new FileTypeValidator();
        }

        [Test]
        public void ContentMatchesExtension_FileStreamParameterNullFileExtensionParameterNull_ThrowsException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _fileTypeValidator.ContentMatchesExtension(null, null); });

            var resultParamName = result.ParamName;

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(FileStreamParameterName, resultParamName);
        }

        [Test]
        public void ContentMatchesExtension_FileStreamParameterNull_ThrowsException()
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _fileTypeValidator.ContentMatchesExtension(null, ValidDocxExtensionWithDot); });

            var resultParamName = result.ParamName;

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(FileStreamParameterName, resultParamName);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ContentMatchesExtension_FileExtensionNullOrEmpty_ThrowsException(string fileExtension)
        {
            var result = Assert.Throws<ArgumentNullException>(delegate { _fileTypeValidator.ContentMatchesExtension(_fileStream, fileExtension); });

            var resultParamName = result.ParamName;

            Assert.IsInstanceOf<ArgumentNullException>(result);
            Assert.AreEqual(FileExtensionParameterName, resultParamName);
        }

        [Test]
        public void ContentMatchesExtension_FileTypeNotValidForFile_ReturnsFalse()
        {
            var result = _fileTypeValidator.ContentMatchesExtension(_fileStream, ValidDocxExtensionWithDot);

            Assert.IsFalse(result);
        }

        [TestCase(ValidDocxExtensionWithDot)]
        [TestCase(ValidDocxExtensionWithoutDot)]
        public void ContentMatchesExtension_FileExtensionValidForFile_ReturnsTrue(string fileExtension)
        {
            var inMemoryFile = GetFileMemoryStream();

            var result = _fileTypeValidator.ContentMatchesExtension(inMemoryFile, fileExtension);

            Assert.IsTrue(result);
        }

        [TestCase(InvalidExtensionWithDot)]
        [TestCase(InvalidExtensionWithoutDot)]
        public void ContentMatchesExtension_FileExtensionNotValidForFile_ReturnsFalse(string fileExtension)
        {
            var inMemoryFile = GetFileMemoryStream();

            var result = _fileTypeValidator.ContentMatchesExtension(inMemoryFile, fileExtension);

            Assert.False(result);
        }

        /// <summary>
        /// Get MemoryStream from test file resource to mock ContentInspector.
        /// </summary>
        private MemoryStream GetFileMemoryStream()
        {
            var inMemoryFile = new MemoryStream();

            string filePath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", ".."));

            using (FileStream fs = File.OpenRead($@"{filePath}{TestFilePath}"))
            {
                fs.CopyTo(inMemoryFile);
            }

            inMemoryFile.Position = 0;

            return inMemoryFile;
        }
    }
}
