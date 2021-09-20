﻿namespace MvcForum.Core.ExtensionMethods
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Constants;
    using Interfaces.Services;
    using Models.General;
    using Providers.Storage;

    public static class HttpExtensionMethods
    {
        /// <summary>
        ///     Checks whether this
        /// </summary>
        /// <param name="file"></param>
        /// <param name="localizationService"></param>
        /// <param name="mustBeImage"></param>
        /// <returns></returns>
        public static FileCheckResult CanBeUploaded(this HttpPostedFileBase file,
            ILocalizationService localizationService, bool mustBeImage = false)
        {
            var result = new FileCheckResult { IsOk = true };

            var fileName = Path.GetFileName(file.FileName);
            if (fileName == null) {
                result.IsOk = false;
                result.Message = localizationService.GetResourceString("Errors.GenericMessage");
                return result;
            }

            // Get the file extension
            var fileExtension = Path.GetExtension(fileName);

            // If can't work out extension then just error
            if (string.IsNullOrWhiteSpace(fileExtension)) {
                result.IsOk = false;
                result.Message = localizationService.GetResourceString("Errors.GenericMessage");
                return result;
            }

            //Before we do anything, check file size
            if (file.ContentLength > Convert.ToInt32(ForumConfiguration.Instance.FileUploadMaximumFileSizeInBytes)) {
                result.IsOk = false;
                result.Message = localizationService.GetResourceString("Post.UploadFileTooBig");
                return result;
            }

            // now check allowed extensions
            var allowedFileExtensions = ForumConfiguration.Instance.FileUploadAllowedExtensions;

            if (file.ContentType.StartsWith("image/")) {
                allowedFileExtensions = Constants.ImageExtensions;
                result.IsImage = true;
            }

            if (!string.IsNullOrWhiteSpace(allowedFileExtensions)) {
                // Turn into a list and strip unwanted commas as we don't trust users!
                var allowedFileExtensionsList = allowedFileExtensions.ToArray(',', true);

                // Remove the dot then check against the extensions in the web.config settings
                fileExtension = fileExtension.TrimStart('.');
                if (!allowedFileExtensionsList.Contains(fileExtension, StringComparer.OrdinalIgnoreCase)) {
                    result.IsOk = false;
                    result.Message = localizationService.GetResourceString("Post.UploadBannedFileExtension");
                    return result;
                }
            } else {
                result.IsOk = false;
                result.Message = "Unable to get allowed extensions";
                return result;
            }

            //  All Good if here
            result.FileName = fileName;
            result.FileExtension = fileExtension;
            return result;
        }

        /// <summary>
        ///     Uploads a file from a posted file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadFolderPath"></param>
        /// <param name="localizationService"></param>
        /// <param name="onlyImages"></param>
        /// <returns></returns>
        public static UploadFileResult UploadFile(this HttpPostedFileBase file, string uploadFolderPath,
            ILocalizationService localizationService, bool onlyImages = false)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid().ToString()}{extension}";
            var upResult = new UploadFileResult { UploadSuccessful = true };
            var storageProvider = StorageProvider.Current;

            var fileOkResult = file.CanBeUploaded(localizationService);

            if (fileOkResult.IsOk) {
                if (!Directory.Exists(uploadFolderPath)) {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                var fileExtension = fileOkResult.FileExtension;

                // Store these here as we may change the values within the image manipulation
                string newFileName;

                // See if this is an image, if so then do some extra checks
                if (fileOkResult.IsImage) {
                    // convert to image and strip metadata
                    var sourceimage = file.ToImage().StripMetaData();

                    newFileName = fileName.CreateFilename();
                    upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, sourceimage.ToMemoryStream());
                } else {
                    // Sort the file name
                    newFileName = fileName.CreateFilename();
                    upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, file.InputStream);
                }

                upResult.UploadedFileName = newFileName;
            } else {
                upResult.UploadSuccessful = false;
                upResult.ErrorMessage = fileOkResult.Message;
            }

            return upResult;
        }
    }
}