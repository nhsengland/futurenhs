using System.Data;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Helpers.Interfaces;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.Azure.Storage.Blob;
using FutureNHS.Api.Services.Validation;
using FutureNHS.Api.DataAccess.Models.FileAndFolder;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FutureNHS.Api.Services
{
    public class FileService : IFileService
    {
        private const string AddFileRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/files/add";
        private const string DownloadFileRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/files/download";
        private const string ViewFileRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/files/view";
        private const string EditFileRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/files/edit";
        private const string VerifiedFileStatus = "Verified";
        private readonly ILogger<DiscussionService> _logger;
        private readonly IFileCommand _fileCommand;
        private readonly IFileBlobStorageProvider _blobStorageProvider;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;
        private readonly IFileTypeValidator _fileTypeValidator;
        private readonly IGroupCommand _groupCommand;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;
        private readonly string[] _acceptedFileTypes = new[] { ".pdf", ".ppt", ".pptx", ".doc", ".docx", ".xls", ".xlsx" };

        public FileService(ISystemClock systemClock, ILogger<DiscussionService> logger, IPermissionsService permissionsService, IFileCommand fileCommand, IFileBlobStorageProvider blobStorageProvider, IFileTypeValidator fileTypeValidator, IGroupCommand groupCommand, IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _systemClock = systemClock;
            _fileCommand = fileCommand;
            _blobStorageProvider = blobStorageProvider;
            _permissionsService = permissionsService;
            _fileTypeValidator = fileTypeValidator;
            _groupCommand = groupCommand;
            _logger = logger;
            _options = options;
        }

        private async Task CreateFileAsync(FileDto fileDto, CancellationToken cancellationToken)
        {
            var now = _systemClock.UtcNow.UtcDateTime;

            var fileStatus = await _fileCommand.GetFileStatus("Verified", cancellationToken);

            fileDto.FileStatus = fileStatus;
            await _fileCommand.CreateFileAsync(fileDto, cancellationToken);
        }

        public async Task<string> GetFileDownloadUrl(Guid userId, string slug, Guid fileId, CancellationToken cancellationToken)
        {
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, DownloadFileRole, cancellationToken);
            if (userCanPerformAction is false)
            {
                _logger.LogError($"Error: DownloadFileAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }
            
            var file = await _fileCommand.GetFileAsync(fileId, VerifiedFileStatus, cancellationToken);

            var downloadUri = _blobStorageProvider.GetRelativeDownloadUrl(file.BlobName, file.FileName, SharedAccessBlobPermissions.Read, cancellationToken);
            return downloadUri;
        }

        public async Task<AuthUser> CheckUserAccess(Guid userId, Guid fileId, string permission, CancellationToken cancellationToken)
        {
            var userAccess = await _fileCommand.GetFileAccess(userId, fileId, cancellationToken);

            if (userAccess == null)
                userAccess = await _fileCommand.GetFileVersionAccess(userId, fileId, cancellationToken);

            var userCanPerformAction = false;

            if (permission == "view")
            {
                userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userAccess.Id, userAccess.GroupSlug, ViewFileRole, cancellationToken);
            }
            else if (permission == "edit")
            {
                userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userAccess.Id, userAccess.GroupSlug, EditFileRole, cancellationToken);
            }

            if (userCanPerformAction is false)
            {
                _logger.LogError($"Error: CheckUserAccess - User:{0} does not have access to group:{1}", userId, userAccess.GroupSlug);
                throw new SecurityException($"Error: User does not have access");
            }

            string? avatar = null;
            if (!userAccess.AvatarUrl.IsNullOrEmpty())
                avatar = $"{_options.Value.PrimaryServiceUrl}/{userAccess.AvatarUrl}";
            
            return new AuthUser 
                {   Id = userAccess.Id, 
                    FullName = userAccess.FullName, 
                    Initials = userAccess.Initials, 
                    EmailAddress = userAccess.EmailAddress,
                    AvatarUrl = avatar
                };
        }

        // TODO Need to figure out how we rollback if cancellation is requested or prevent it?
        public async Task UploadFileMultipartDocument(Guid userId, string slug, Guid folderId, Stream requestBody, string? contentType, CancellationToken cancellationToken)
        {
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, AddFileRole, cancellationToken);

            if (userCanPerformAction is false)
            {
                _logger.LogError($"Error: CreateFileAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var file = await this.UploadMultipartContent(requestBody, contentType, cancellationToken);

            file.CreatedBy = userId;
            file.ParentFolder = folderId;

            var validator = new FileValidator();
            var validationResult = await validator.ValidateAsync(file, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            try
            {
                await CreateFileAsync(file, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: CreateFileAsync - Error adding file to database");
                await _blobStorageProvider.DeleteFileAsync(file.BlobName);
            }
        }

        /// based on microsoft example https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/mvc/models/file-uploads/samples/
        /// and large file streaming example https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#upload-large-files-with-streaming
        private async Task<FileDto> UploadMultipartContent(Stream requestBody, string? contentType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if HttpRequest (Form Data) is a Multipart Content Type
            if (!MultipartRequestHelper.IsMultipartContentType(contentType))
            {
                throw new InvalidDataException($"Expected a multipart request, but got {contentType}");
            }
            var defaultFormOptions = new FormOptions();
            // Create a Collection of KeyValue Pairs.
            var formAccumulator = new KeyValueAccumulator();
            // Determine the Multipart Boundary.
            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(contentType), defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, requestBody);
            var section = await reader.ReadNextSectionAsync(cancellationToken);
            var fileDto = new FileDto();

            // Loop through each 'Section', starting with the current 'Section'.
            while (section != null)
            {
                // Check if the current 'Section' has a ContentDispositionHeader.
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        if (contentDisposition != null)
                        {
                            var sectionFileName = contentDisposition.FileName.Value;
                            // use an encoded filename in case there is anything weird
                            var encodedFileName = WebUtility.HtmlEncode(Path.GetFileName(sectionFileName));

                            // read the section filename to get the content type
                            var fileExtension = Path.GetExtension(sectionFileName);

                            // now make it unique
                            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                            string? md5Hash;

                            // TODO MimeDetective does not work when stream has already been uploaded - figure out a solution
                            //if (fileContentTypeMatchesExtension is false)
                            //{
                            //    await _blobStorageProvider.DeleteFileAsync(uniqueFileName);
                            //    _logger.LogError("File extension:{0} does not match the file signature", fileExtension);
                            //    throw new FormatException("File extension} does not match the file signature");
                            //}

                            if (!_acceptedFileTypes.Contains(fileExtension.ToLower()))
                            {
                                _logger.LogError("file extension:{0} is not an accepted file", fileExtension);
                                throw new ConstraintException("The file is not an accepted file");
                            }
                            try
                            {
                                md5Hash = await _blobStorageProvider.UploadFileAsync(section.Body, uniqueFileName, MimeTypesMap.GetMimeType(encodedFileName), cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred uploading file to blob storage");
                                throw;
                            }

                            // TODO MimeDetective does not work when stream has already been uploaded - figure out a solution
                            //if (fileContentTypeMatchesExtension is false)
                            //{
                            //    await _blobStorageProvider.DeleteFileAsync(uniqueFileName);
                            //    _logger.LogError("File extension:{0} does not match the file signature", fileExtension);
                            //    throw new FormatException("File extension} does not match the file signature");
                            //}

                            var now = _systemClock.UtcNow.UtcDateTime;

                            // trick to get the size without reading the stream in memory
                            var size = section.Body.Position;

                            fileDto.FileName = encodedFileName;
                            fileDto.FileExtension = fileExtension;
                            fileDto.FileSizeBytes = size;
                            fileDto.BlobName = uniqueFileName;
                            fileDto.CreatedAtUTC = now;
                            if (md5Hash != null) 
                                fileDto.BlobHash = Convert.FromBase64String(md5Hash);
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // if for some reason other form data is sent it would get processed here
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition?.Name.ToString().ToLowerInvariant());

                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(section.Body, encoding, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
                        {
                            var value = await streamReader.ReadToEndAsync();
                            if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }
                            formAccumulator.Append(key.Value, value);
                            if (formAccumulator.ValueCount > defaultFormOptions.ValueCountLimit)
                            {
                                _logger.LogError("FileUpload: Form key count limit {0} exceeded.", defaultFormOptions.ValueCountLimit);
                                throw new FormatException($"Form key count limit { defaultFormOptions.ValueCountLimit } exceeded.");
                            }
                        }
                    }
                }
                // Begin reading the next 'Section' inside the 'Body' of the Request.
                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            if (formAccumulator.HasValues)
            {
                var formValues = formAccumulator.GetResults();
                var titleFound = formValues.TryGetValue("title", out var title);
                if (titleFound is false)
                {
                    throw new ArgumentNullException($"Title was not provided");
                }
                var descriptionFound = formValues.TryGetValue("description", out var description);
                if (descriptionFound is false)
                {
                    throw new ArgumentNullException($"Description was not provided");
                }

                fileDto.Title = title;
                fileDto.Description = description;
            }
            return fileDto;
        }


        private static Encoding? GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

    }
}
