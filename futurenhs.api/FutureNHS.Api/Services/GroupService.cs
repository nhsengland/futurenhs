using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Helpers.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Validation;
using Ganss.XSS;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Data;
using System.Net;
using System.Security;
using System.Text;

namespace FutureNHS.Api.Services
{
    public class GroupService : IGroupService
    {
        private const string GroupEditRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/edit";
        private const string GroupInviteDeleteRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/view";
        private const string GroupInviteViewRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/view";
        private const string GroupViewRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/view";
        private const string AdminViewRole = $"https://schema.collaborate.future.nhs.uk/admin/v1/view";
        private readonly ILogger<DiscussionService> _logger;
        private readonly IImageBlobStorageProvider _blobStorageProvider;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;
        private readonly IFileTypeValidator _fileTypeValidator;
        private readonly IGroupCommand _groupCommand;
        private readonly IGroupImageService _imageService;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IContentService _contentService;



        private readonly string[] _acceptedFileTypes = new[] { ".png", ".jpg", ".jpeg" };
        private const long MaxFileSizeBytes = 5242880; // 5MB

        public GroupService(ISystemClock systemClock,
            ILogger<DiscussionService> logger,
            IPermissionsService permissionsService,
            IFileCommand fileCommand,
            IImageBlobStorageProvider blobStorageProvider,
            IFileTypeValidator fileTypeValidator,
            IGroupImageService imageService,
            IGroupCommand groupCommand,
            IHtmlSanitizer htmlSanitizer, 
            IGroupDataProvider groupDataProvider, 
            IContentService contentService)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _blobStorageProvider = blobStorageProvider ?? throw new ArgumentNullException(nameof(blobStorageProvider));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _fileTypeValidator = fileTypeValidator ?? throw new ArgumentNullException(nameof(fileTypeValidator));
            _groupCommand = groupCommand ?? throw new ArgumentNullException(nameof(groupCommand));
            _groupDataProvider = groupDataProvider ?? throw new ArgumentNullException(nameof(groupDataProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer));
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
        }

        public async Task<(uint totalGroups, IEnumerable<AdminGroupSummary> groupSummaries)> AdminGetGroupsAsync(Guid userId, uint page = PaginationSettings.MinOffset, uint pageSize = PaginationSettings.DefaultPageSize, CancellationToken cancellationToken = default)
        {
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, AdminViewRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: AdminGetGroupsAsync - User:{0} does not have access to view groups", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            var groups = await _groupDataProvider.AdminGetGroupsAsync(page, pageSize, cancellationToken);

            return groups;
        }

        public async Task<GroupData?> GetGroupAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupEditRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: GetGroupAsync - User:{0} does not have access to edit/delete group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var group = await _groupCommand.GetGroupAsync(slug, cancellationToken);

            return group;
        }

        private async Task UpdateGroupAsync(Guid userId, string slug, GroupDto groupDto, CancellationToken cancellationToken)
        {

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupEditRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: UpdateGroupAsync - User:{0} does not have access to edit group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var now = _systemClock.UtcNow.UtcDateTime;

            await _groupCommand.UpdateGroupAsync(groupDto, cancellationToken);
        }
        
        public async Task DeleteGroupInviteAsync(Guid groupInviteId, Guid userId, byte[] rowVersion, CancellationToken cancellationToken)
        {

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, groupInviteId, GroupInviteDeleteRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: DeleteGroupInviteAsync - User:{0} does not have access to delete group invite:{1}", userId, groupInviteId);
                throw new SecurityException($"Error: User does not have access");
            }
            
            var groupInvite = await _groupCommand.GetGroupInviteAsync(groupInviteId, userId, cancellationToken);
            // if (!groupInvite.RowVersion.SequenceEqual(rowVersion))
            // {
            //     _logger.LogError($"Precondition Failed: DeleteGroupInviteAsync - GroupInvite:{0} has changed prior to submission ", groupInviteId);
            //     throw new PreconditionFailedExeption("Precondition Failed: GroupUser has changed prior to submission");
            // }

        }

        public async Task<GroupInvite> GetGroupInviteAsync(Guid groupInviteId, Guid userId, CancellationToken cancellationToken)
        {

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, groupInviteId, GroupInviteViewRole, cancellationToken);
            if (userCanPerformAction is not true)
            {
                _logger.LogError($"Error: GetGroupInviteAsync - User:{0} does not have access to view group invite:{1}", userId, groupInviteId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _groupCommand.GetGroupInviteAsync(groupInviteId, userId, cancellationToken);
        }


        // TODO Need to figure out how we rollback if cancellation is requested or prevent it?
        public async Task UpdateGroupMultipartDocument(Guid userId, string slug, byte[] rowVersion, Stream requestBody, string? contentType, CancellationToken cancellationToken)
        {
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupEditRole, cancellationToken);

            if (userCanPerformAction is false)
            {
                _logger.LogError($"Error: CreateFileAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var groupDto = await _groupCommand.GetGroupAsync(slug, cancellationToken);
            if (!groupDto.RowVersion.SequenceEqual(rowVersion))
            {
                _logger.LogError($"Precondition Failed: UpdateGroupAsync - Group:{0} has changed prior to submission ", slug);
                throw new PreconditionFailedExeption("Precondition Failed: Group has changed prior to submission");
            }

            var (group, image) = await UploadGroupImageMultipartContent(groupDto, userId, slug, requestBody, rowVersion, contentType, cancellationToken);

            var groupValidator = new GroupValidator();
            var groupValidationResult = await groupValidator.ValidateAsync(group, cancellationToken);
            if (groupValidationResult.Errors.Count > 0)
                throw new ValidationException(groupValidationResult);

            try
            {
                if (image is not null)
                {
                    var imageId = await _imageService.CreateImageAsync(image);
                    group = group with { ImageId = imageId };
                }
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: CreateImageAsync - Error adding image to group {0}", slug);
                if (image is not null)
                {
                    await _blobStorageProvider.DeleteFileAsync(image.FileName);
                    await _imageService.DeleteImageAsync(image.Id);
                }
            }
            try
            {
                await UpdateGroupAsync(userId, slug, group, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: UpdateGroupAsync - Error updating group {0}", slug);
                if (image is not null)
                {
                    await _blobStorageProvider.DeleteFileAsync(image.FileName);
                    await _imageService.DeleteImageAsync(image.Id);
                }
            }

            //TODO - Delete old image of everything succeeds and the image has been removed or replaced 


        }

        /// based on microsoft example https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/mvc/models/file-uploads/samples/
        /// and large file streaming example https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#upload-large-files-with-streaming
        private async Task<(GroupDto, ImageDto?)> UploadGroupImageMultipartContent(GroupData groupData, Guid userId, string slug, Stream requestBody, byte[] rowVersion, string? contentType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if HttpRequest (Form Data) is a Multipart Content Type
            if (!MultipartRequestHelper.IsMultipartContentType(contentType))
            {
                throw new InvalidDataException($"Expected a multipart request, but got {contentType}");
            }

            var now = _systemClock.UtcNow.UtcDateTime;

            var defaultFormOptions = new FormOptions();
            // Create a Collection of KeyValue Pairs.
            var formAccumulator = new KeyValueAccumulator();
            // Determine the Multipart Boundary.
            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(contentType), defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, requestBody);
            var section = await reader.ReadNextSectionAsync(cancellationToken);
            ImageDto? imageDto = null;
            GroupDto groupDto = new GroupDto();

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

                            if (!_acceptedFileTypes.Contains(fileExtension.ToLower()))
                            {
                                _logger.LogError("file extension:{0} is not an accepted image file", fileExtension);
                                throw new ValidationException("Image", "The image is not in an accepted format");
                            }

                            var compressedImage = _imageService.TransformImageForGroupHeader(section.Body);

                            try
                            {
                                await _blobStorageProvider.UploadFileAsync(compressedImage.Image, uniqueFileName,
                                    MimeTypesMap.GetMimeType(encodedFileName), cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred uploading file to blob storage");
                                throw;
                            }

                            // trick to get the size without reading the stream in memory
                            var size = section.Body.Position;

                            // TODO MimeDetective does not work when stream has already been uploaded - figure out a solution
                            //if (fileContentTypeMatchesExtension is false)
                            //{
                            //    await _blobStorageProvider.DeleteFileAsync(uniqueFileName);
                            //    _logger.LogError("File extension:{0} does not match the file signature", fileExtension);
                            //    throw new FormatException("File extension} does not match the file signature");
                            //}

                            imageDto = new ImageDto
                            {
                                FileSizeBytes = size,
                                FileName = uniqueFileName,
                                Height = compressedImage.Height,
                                Width = compressedImage.Width,
                                IsDeleted = false,
                                MediaType = compressedImage.MediaType,
                                CreatedBy = userId,
                                CreatedAtUtc = now
                            };

                            var imageValidator = new ImageValidator(MaxFileSizeBytes);
                            var imageValidationResult = await imageValidator.ValidateAsync(imageDto, cancellationToken);
                            if (imageValidationResult.Errors.Count > 0)
                            {
                                await _blobStorageProvider.DeleteFileAsync(uniqueFileName);
                                _logger.LogError("File size:{0} is greater than the max allowed size:{1}", size, MaxFileSizeBytes);
                                throw new ValidationException(imageValidationResult);
                            }
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
                                _logger.LogError("GroupEdit: Form key count limit {0} exceeded.", defaultFormOptions.ValueCountLimit);
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

                // Get values from multipart form
                var nameFound = formValues.TryGetValue("name", out var name);
                if (nameFound is false)
                {
                    throw new ValidationException("Name", "Name was not provided");
                }
                var straplineFound = formValues.TryGetValue("strapline", out var strapline);
                if (straplineFound is false)
                {
                    throw new ValidationException("Strapline", "Strapline was not provided");
                }

                formValues.TryGetValue("themeid", out var theme);
                if (Guid.TryParse(theme, out var themeId) is false || themeId == new Guid())
                {
                    throw new ValidationException(nameof(GroupDto.ThemeId), "Theme was not provided");
                }

                formValues.TryGetValue("imageid", out var image);
                var imageId = Guid.TryParse(image, out var imageGuid) ? (Guid?)imageGuid : null;
                if (imageId.HasValue)
                {
                    if (imageId == new Guid())
                    {
                        throw new ArgumentOutOfRangeException($"Incorrect Id provided");
                    }
                }

                formValues.TryGetValue("isPublic", out var publicGroup);
                var isPublic = bool.TryParse(publicGroup, out var isPublicBool) ? isPublicBool : false;
                if (!groupData.IsPublic)
                {
                    if (isPublic != groupData.IsPublic)
                    {
                        throw new ValidationException("isPublic", "Cannot make a private group public");
                    }
                }

                groupDto = new GroupDto
                {
                    Slug = slug,
                    Name = _htmlSanitizer.Sanitize(name),
                    Strapline = _htmlSanitizer.Sanitize(strapline),
                    ThemeId = themeId,
                    ImageId = imageId,
                    ModifiedBy = userId,
                    ModifiedAtUtc = now,
                    IsPublic = isPublic,
                    RowVersion = rowVersion
                };
            }
            return (groupDto, imageDto);
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

        public async Task<(uint, IEnumerable<GroupMember>)> GetGroupMembersAsync(Guid userId, string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupViewRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetGroupMembersAsync - User:{0} does not have permission to get members of this group:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            return await _groupDataProvider.GetGroupMembersAsync(slug, offset, limit, sort, cancellationToken);
        }

        public async Task<(uint, IEnumerable<PendingGroupMember>)> GetPendingGroupMembersAsync(Guid userId, string slug, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupViewRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetPendingGroupMembersAsync - User:{0} does not have permission to get pending group members of this group:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            return await _groupDataProvider.GetPendingGroupMembersAsync(slug, offset, limit, sort, cancellationToken);
        }

        public async Task<Group?> GetGroupAsync(string slug, Guid userId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            return await _groupDataProvider.GetGroupAsync(slug, userId, cancellationToken);
        }

        public async Task<GroupMemberDetails> GetGroupMemberAsync(Guid userId, string slug, Guid memberId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentOutOfRangeException(nameof(slug));
            if (Guid.Empty == memberId) throw new ArgumentOutOfRangeException(nameof(memberId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupViewRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetGroupMemberAsync - User:{0} does not have permission to get group member:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            return await _groupDataProvider.GetGroupMemberAsync(slug, memberId, cancellationToken);
        }

        public async Task<GroupSite> CreateGroupSiteDataAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupEditRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError("Error: CreateGroupSiteDataAsync - User: {User} does not have permission to create missing group site for group: {GroupSlug}", userId, slug);
                throw new NotFoundException($"Site data for group slug: {slug} is missing");
            }

            var now = _systemClock.UtcNow.UtcDateTime;
            var group = await _groupCommand.GetGroupAsync(slug, cancellationToken);
            var createContentResponse = await _contentService.CreatePageAsync(userId, group.Id, null, cancellationToken);
            var createdContentGuid = Guid.Parse(createContentResponse.Data);

            var groupSiteDto = new GroupSiteDto()
            {
                GroupId = group.Id,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ContentRootId = createdContentGuid
            };

            await _groupCommand.CreateGroupSiteAsync(groupSiteDto, cancellationToken);

            return await _groupDataProvider.GetGroupSiteDataAsync(slug, cancellationToken);
        }

        public async Task<GroupSite> GetGroupSiteDataAsync(Guid userId, string slug, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, slug, GroupViewRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetGroupSiteDataAsync - User:{0} does not have permission to get group site for group:{1}(slug)", userId, slug);
                throw new ForbiddenException($"Error: User does not have access");
            }

            return await _groupDataProvider.GetGroupSiteDataAsync(slug, cancellationToken);
        }

        public async Task<(uint totalGroups, IEnumerable<GroupSummary> groupSummaries)> GetGroupsForUserAsync(Guid userId, bool isMember, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, GroupViewRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetGroupsForUserAsync - User:{0} does not have permission to get groups for user", userId);
                throw new ForbiddenException($"Error: User does not have access");
            }

            return await _groupDataProvider.GetGroupsForUserAsync(userId, isMember, offset, limit, cancellationToken);
        }
        
        public async Task<(uint totalGroups, IEnumerable<GroupInviteSummary> groupSummaries)> GroupInvitesForUserAsync(Guid userId, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, GroupViewRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetGroupsForUserAsync - User:{0} does not have permission to get groups for user", userId);
                throw new ForbiddenException($"Error: User does not have access");
            }

            return await _groupDataProvider.GetGroupInvitesForUserAsync(userId, offset, limit, cancellationToken);
            
        }

    }
}
