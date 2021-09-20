﻿//-----------------------------------------------------------------------
// <copyright file="FileUploadValidationService.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Services
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    public class FileUploadValidationService : IFileUploadValidationService
    {
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="localizationService"></param>
        public FileUploadValidationService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <summary>
        /// Validate an uploaded file.
        /// </summary>
        /// <param name="file">HttpPostedFileBase to validate</param>
        /// <param name="simpleValidation">bool - perform simple or complex validation.</param>
        /// <returns></returns>
        public UploadBlobResult ValidateUploadedFile(HttpPostedFileBase file, bool simpleValidation)
        {
            var result = new UploadBlobResult
            {
                ValidationErrors = GetFileValidationErrors(file, simpleValidation)
            };
            return result;
        }

        /// <summary>
        /// Get any file validation errors.
        /// </summary>
        /// <param name="file">File to validate</param>
        /// <param name="simpleValidation">Determine if we need to perform complex validation i.e. using MIME DEtective.</param>
        /// <returns></returns>
        private List<string> GetFileValidationErrors(HttpPostedFileBase file, bool simpleValidation = true)
        {
            var validationErrors = new List<string>
            {
                ValidateActualMimeType(file, simpleValidation),
                ValidateFileTypeAllowed(Path.GetExtension(file.FileName)),
                ValidateFileSize(file.ContentLength),
                ValidateFilenameSet(Path.GetFileNameWithoutExtension(file.FileName)),
                ValidateFilenameLength(file.FileName)
            };

            // Remove empty values, if any of the above valid empty entries will be added
            validationErrors = validationErrors.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            return validationErrors;
        }

        /// <summary>
        /// Determine if a valid MIME Type uploaded, only perform if complex validation required.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="simpleValidation"></param>
        /// <returns></returns>
        private string ValidateActualMimeType(HttpPostedFileBase file, bool simpleValidation)
        {
            if (!simpleValidation)
            {
                //TODO - implement MIME-Detective file type validation
            }
            return null;
        }

        /// <summary>
        /// Validate that the type of file is allowed to be uploaded.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private string ValidateFileTypeAllowed(string extension)
        {
            return !AllowedFileTypes.Contains(extension) ? _localizationService.GetResourceString("File.Error.InvalidType") : null; 
        }

        /// <summary>
        /// Validate that the size of the file does not exceed the maximum permitted.
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        private string ValidateFileSize(int fileSize)
        {
            return fileSize > MaxFileSize ? _localizationService.GetResourceString("File.Error.InvalidSize") : null;
        }

        /// <summary>
        /// Validate that the file name is set and is not empty e.g. not ".docx".
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string ValidateFilenameSet(string filename)
        {
            return filename == string.Empty ? _localizationService.GetResourceString("File.Error.NullName") : null;
        }

        /// <summary>
        /// Validate that the length of the file name does not exceed the maximum allowed.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string ValidateFilenameLength(string filename)
        {
            return filename.Length > MaxFilenameLength ? _localizationService.GetResourceString("File.Error.NameLength") : null;
        }

        /// <summary>
        /// Get maximum file size allowed from config file.
        /// </summary>
        private int MaxFileSize => ConfigUtils.GetAppSettingInt32("FileUpload_FileLimitBytes", 250000000);

        /// <summary>
        /// Get maximum file name length allowed from config file.
        /// </summary>
        private int MaxFilenameLength => ConfigUtils.GetAppSettingInt32("FileUpload_MaxFilenameLength", 100);

        /// <summary>
        /// Get allowed file types from config file.
        /// </summary>
        private List<string> AllowedFileTypes
        {
            get
            {
                var allowedTypes = new List<string>();
                var configValue = ConfigUtils.GetAppSetting("FileUpload_ValidFileTypes", string.Empty);

                if (!string.IsNullOrWhiteSpace(configValue))
                {
                    allowedTypes.AddRange(configValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                }

                return allowedTypes;
            }
        }
    }
}
