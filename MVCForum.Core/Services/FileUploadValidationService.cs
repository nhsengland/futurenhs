namespace MvcForum.Core.Services
{
    using MimeDetective;
    using MvcForum.Core.Interfaces.Helpers;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    public sealed class FileUploadValidationService : IFileUploadValidationService
    {
        private const int TwoHundredAndFiftyMb = 250000000;
        private const int OneHundredCharacters = 100;

        private readonly IValidateFileType _fileTypeValidator;

        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Maximum file size allowed - from config file.
        /// </summary>
        private readonly int _maxFileSizeInBytes;

        /// <summary>
        /// Maximum file name length allowed - from config file.
        /// </summary>
        private readonly int _maxFilenameLength;

        /// <summary>
        /// valid file types allowed - from config file.
        /// </summary>
        private readonly string[] _validFileTypes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="localizationService"></param>
        public FileUploadValidationService(ILocalizationService localizationService, IValidateFileType fileTypeValidator)
        {
            if (localizationService is null) throw new ArgumentNullException(nameof(localizationService));
            if (fileTypeValidator is null) throw new ArgumentNullException(nameof(fileTypeValidator)); 

            _localizationService = localizationService;
            _fileTypeValidator = fileTypeValidator;

            _maxFileSizeInBytes = ConfigUtils.GetAppSettingInt32("FileUpload_FileLimitBytes", TwoHundredAndFiftyMb);
            _maxFilenameLength = ConfigUtils.GetAppSettingInt32("FileUpload_MaxFilenameLength", OneHundredCharacters);

            var configValidFileTypes = ConfigUtils.GetAppSetting("FileUpload_ValidFileTypes", string.Empty);
            _validFileTypes = configValidFileTypes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Get any file validation errors.
        /// </summary>
        /// <param name="file">File to validate</param>
        /// <returns></returns>
        public ValidateBlobResult ValidateUploadedFile(HttpPostedFileBase file)
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            var actualMimeType = default(string);
            var validationErrors = new List<string>();

            // Simple file type validation
            if (FileTypeIsNotSupported(Path.GetExtension(file.FileName)))
            {
                validationErrors.Add(_localizationService.GetResourceString("File.Error.InvalidType"));
            }
            else
            {
                // Passed simple type validation, perform mime type validation and identify mime type
                var fileExtension = Path.GetExtension(file.FileName);
                var fileContent = file.InputStream;

                if (_fileTypeValidator.ContentMatchesExtension(fileContent, fileExtension))
                {
                    actualMimeType = MimeMapping.GetMimeMapping(fileExtension);
                }
                else
                {
                    validationErrors.Add(_localizationService.GetResourceString("File.Error.InvalidType"));
                }
            }

            // Perform other validation required

            if (FileSizeIsOutOfRange(file.ContentLength))
            {
                validationErrors.Add(_localizationService.GetResourceString("File.Error.InvalidSize"));
            }

            if (FilenameIsEmpty(Path.GetFileNameWithoutExtension(file.FileName)))
            {
                validationErrors.Add(_localizationService.GetResourceString("File.Error.NullName"));
            }

            if (FilenameLengthIsOutOfRange(file.FileName))
            {
                validationErrors.Add(_localizationService.GetResourceString("File.Error.NameLength"));
            }

            return new ValidateBlobResult()
            {
                ValidationErrors = validationErrors,
                MimeType = actualMimeType
            };
        }

        /// <summary>
        /// Validate if the file type is valid for upload.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private bool FileTypeIsNotSupported(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension)) return false;

            return Array.Find(_validFileTypes, _ => _.Equals(extension, StringComparison.OrdinalIgnoreCase)) is null;
        }

        /// <summary>
        /// Validate the file size does not exceed the maximum permitted.
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        private bool FileSizeIsOutOfRange(int fileSize)
        {
            return fileSize > _maxFileSizeInBytes;
        }

        /// <summary>
        /// Validate the file name is set and not empty e.g. not ".docx".
        /// This meets the acceptance criteria where this scenario is not allowed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool FilenameIsEmpty(string filename)
        {
            return filename == string.Empty;
        }

        /// <summary>
        /// Validate the file name length does not exceed the maximum allowed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool FilenameLengthIsOutOfRange(string filename)
        {
            return filename.Length > _maxFilenameLength;
        }
    }
}
