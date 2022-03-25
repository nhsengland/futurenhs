namespace MvcForum.Core.ExtensionMethods
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Constants;
    using Interfaces.Services;
    using Models.General;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
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
            ILocalizationService localizationService, IImageCommand imageCommand = null, IImageRepository imageRepository = null,  IImageService imageService = null, 
            bool isAvatar = false, Guid imageSaveId = default(Guid))
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var upResult = new UploadFileResult { UploadSuccessful = true };
            var storageProvider = StorageProvider.Current;

            var fileOkResult = file.CanBeUploaded(localizationService);

            if (fileOkResult.IsOk) {
                if (!Directory.Exists(uploadFolderPath)) {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                var fileExtension = fileOkResult.FileExtension;

                // Store these here as we may change the values within the image manipulation
                string newFileName = string.Empty;

                // See if this is an image, if so then do some extra checks
                if (fileOkResult.IsImage) 
                {
                    var sourceimage = file.ToImage();

                    if (!(imageService is null) && !(imageRepository is null))
                    {
                        var newFileId = imageSaveId != default(Guid) ? imageSaveId : Guid.NewGuid();
                        newFileName = $"{newFileId}.webp";

                        var transformedImage = isAvatar ? imageService.TransformImageForAvatar(new Bitmap(sourceimage)) :
                                                          imageService.TransformImageForGroupHeader(new Bitmap(sourceimage));
                        
                        var imageStream = new MemoryStream();
                        imageStream.Write(transformedImage.Bytes, 0, transformedImage.Bytes.Length);
                        upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, imageStream);

                        var imageVM = new ImageViewModel(upResult.UploadedFileName, "user avatar")
                        {
                            FileName = newFileName,
                            FileSizeBytes = Convert.ToInt32(file.ContentLength),
                            Height = transformedImage.Height,
                            Width = transformedImage.Width,
                            MediaType = transformedImage.MediaType
                        };

                        if (isAvatar)
                        {
                            var membershipUserImageId = imageRepository.GetMembershipUserImageId(imageSaveId);
                            if (!(membershipUserImageId is null))
                            {
                                imageVM.Id = (Guid)membershipUserImageId;
                                _ = imageCommand.Update(imageVM);
                            }
                            else
                            {
                                var imageId = imageCommand.Create(imageVM);
                                _ = imageCommand.UpdateMembershipUserImageId(imageSaveId, imageId, newFileName);
                            }
                        }
                        else
                        {
                            var groupImageId = imageRepository.GetGroupImageId(imageSaveId);
                            if (!(groupImageId is null))
                            {
                                imageVM.Id = (Guid)groupImageId;
                                _ = imageCommand.Update(imageVM);
                            }
                            else
                            {
                                var imageId = imageCommand.Create(imageVM);
                                _ = imageCommand.UpdateGroupImageId(imageSaveId, imageId, newFileName);
                            }
                        }
                    }
                    else
                    {
                        sourceimage.StripMetaData();
                        newFileName = fileName.CreateFilename();
                        upResult.UploadedFileUrl = storageProvider.SaveAs(uploadFolderPath, newFileName, sourceimage.ToMemoryStream());
                    }
                } 
                else 
                {
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