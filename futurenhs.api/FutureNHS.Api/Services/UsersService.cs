using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.DataAccess.Storage.Providers.Interfaces;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Validation;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;

namespace FutureNHS.Api.Services
{
    public class UserService : IUserService
    {
        private const string ListMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/list";
        private const string AddMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/add";
        private const string EditMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/edit";

        private readonly string _fqdn;
        private readonly ILogger<UserService> _logger;
        private readonly IUserAdminDataProvider _userAdminDataProvider;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;
        private readonly IUserCommand _userCommand;
        private readonly IEmailService _emailService;
        private readonly IUserImageService _imageService;
        private readonly IImageBlobStorageProvider _blobStorageProvider;
        private readonly IUserDataProvider _userDataProvider;


        private readonly string[] _acceptedFileTypes = new[] { ".png", ".jpg", ".jpeg" };
        private const long MaxFileSizeBytes = 500000; // 500kb

        // Notification template Ids
        private readonly string _registrationEmailId;

        public UserService(ILogger<UserService> logger, ISystemClock systemClock, IPermissionsService permissionsService, IUserAdminDataProvider userAdminDataProvider, IUserCommand userCommand, IEmailService emailService, IOptionsSnapshot<GovNotifyConfiguration> notifyConfig, IOptionsSnapshot<ApplicationGateway> gatewayConfig, IUserImageService imageService, IImageBlobStorageProvider blobStorageProvider, IUserDataProvider userDataProvider)
        {
            _permissionsService = permissionsService;
            _userAdminDataProvider = userAdminDataProvider;
            _systemClock = systemClock;
            _logger = logger;
            _userCommand = userCommand;
            _emailService = emailService;
            _fqdn = gatewayConfig.Value.FQDN;
            _imageService = imageService;
            _blobStorageProvider = blobStorageProvider;

            // Notification template Ids
            _registrationEmailId = notifyConfig.Value.RegistrationEmailTemplateId;
            _userDataProvider = userDataProvider;
        }

        public async Task<(uint, IEnumerable<Member>)> GetMembersAsync(Guid userId, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, ListMembersRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateDiscussionAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _userAdminDataProvider.GetMembersAsync(offset, limit, sort, cancellationToken);
        }

        public async Task InviteMemberToGroupAndPlatformAsync(Guid userId, Guid? groupId, string email, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, AddMembersRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: InviteMemberToGroupAndPlatformAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException($"Email was not provided");
            }

            if (email.Length > 254)
            {
                throw new ArgumentOutOfRangeException($"Email must be less than 254 characters");
            }

            MailAddress emailAddress;
            try
            {
                emailAddress = new MailAddress(email);
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException($"Email is not in a valid format");
            }

            var userInvite = new GroupInviteDto
            {
                EmailAddress = emailAddress.Address.ToLowerInvariant(),
                GroupId = groupId,
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,

            };

            var registrationLink = CreateRegistrationLink();

            var personalisation = new Dictionary<string, dynamic>
            {
                {"registration_link", registrationLink}
            };

            await _userCommand.CreateInviteUserAsync(userInvite, cancellationToken);
            await _emailService.SendEmailAsync(emailAddress, _registrationEmailId, personalisation);
        }

        public async Task UpdateMemberAsync(Guid userId, Stream requestBody, string? contentType, byte[] rowVersion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, EditMembersRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: UpdateMemberAsync- User:{0} does not have access to perform edit actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            var (user, image) = await UploadUserImageMultipartContent(userId, requestBody, rowVersion, contentType, cancellationToken);

            if (user is not null)
            {
                var userValidator = new UserValidator();
                var userValidationResult = await userValidator.ValidateAsync(user, cancellationToken);
                if (userValidationResult.Errors.Count > 0)
                    throw new ValidationException(userValidationResult);
            }

            if (image is not null)
            {
                var imageValidator = new ImageValidator();
                var imageValidationResult = await imageValidator.ValidateAsync(image, cancellationToken);
                if (imageValidationResult.Errors.Count > 0)
                    throw new ValidationException(imageValidationResult);
            }

            try
            {
                if (image is not null)
                {
                    var imageId = await _imageService.CreateImageAsync(image);
                    user = user with { ImageId = imageId };
                }
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: CreateImageAsync - Error adding image to user {0}");
                if (image is not null)
                {
                    await _blobStorageProvider.DeleteFileAsync(image.FileName);
                    await _imageService.DeleteImageAsync(image.Id);
                }
            }

            try
            {
                await _userCommand.UpdateUserAsync(user, rowVersion, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: UpdateUserAsync - Error updating user {0}");
            }

        }

        private string CreateRegistrationLink()
        {
            var registrationLink = $"{_fqdn}/members/register";
            return registrationLink;
        }

        private async Task<(MemberDto, ImageDto?)> UploadUserImageMultipartContent(Guid userId, Stream requestBody, byte[] rowVersion, string? contentType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var now = _systemClock.UtcNow.UtcDateTime;

            var defaultFormOptions = new FormOptions();
            // Create a Collection of KeyValue Pairs.
            var formAccumulator = new KeyValueAccumulator();
            // Determine the Multipart Boundary.
            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(contentType), defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, requestBody);
            var section = await reader.ReadNextSectionAsync(cancellationToken);
            ImageDto? imageDto = null;
            MemberDto userDto = new MemberDto();

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

                            var compressedImage = _imageService.TransformImageForAvatar(section.Body);

                            if (!_acceptedFileTypes.Contains(fileExtension.ToLower()))
                            {
                                _logger.LogError("file extension:{0} is not an accepted file", fileExtension);
                                throw new ConstraintException("The file is not an accepted file");
                            }
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

                            // check size limit in case somehow a larger file got through. we can't do it until after the upload because we don't want to put the stream in memory
                            if (MaxFileSizeBytes < size)
                            {
                                await _blobStorageProvider.DeleteFileAsync(uniqueFileName);
                                _logger.LogError("File size:{0} is greater than the max allowed size:{1}", size, MaxFileSizeBytes);
                                throw new ConstraintException("File size is greater than the max allowed size");
                            }

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
                                _logger.LogError("UserEdit: Form key count limit {0} exceeded.", defaultFormOptions.ValueCountLimit);
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
                var idFound = formValues.TryGetValue("id", out var id);

                if (idFound is false || string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException($"Id was not provided");
                }
                var updateUserId = Guid.TryParse(id, out var idGuid) ? (Guid?)idGuid : null;
                if (updateUserId.HasValue)
                {
                    if (updateUserId == new Guid())
                    {
                        throw new ArgumentOutOfRangeException($"Incorrect Id provided");
                    }

                }

                // Check if users been updated
                // Unable to place in controller due to disabling form value model binding
                var user = await _userCommand.GetMemberAsync(idGuid, cancellationToken);
                if (!user.RowVersion.SequenceEqual(rowVersion))
                {
                    _logger.LogError($"Precondition Failed: UpdateUserAsync - User:{0} has changed prior to submission", updateUserId);
                    throw new PreconditionFailedExeption("Precondition Failed: User has changed prior to submission");
                }

                var firstNameFound = formValues.TryGetValue("firstName", out var firstName);
                if (firstNameFound is false || string.IsNullOrEmpty(firstName))
                {
                    throw new ArgumentNullException($"First name was not provided");
                }
                var lastNameFound = formValues.TryGetValue("lastName", out var surname);
                if (lastNameFound is false)
                {
                    throw new ArgumentNullException($"Last name was not provided");
                }
                var pronoundsFound = formValues.TryGetValue("pronouns", out var pronouns);
                if (pronoundsFound is false)
                {
                    throw new ArgumentNullException($"Pronouns were not provided");
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

                userDto = new MemberDto
                {
                    Id = updateUserId,
                    FirstName = firstName,
                    Surname = surname,
                    Pronouns = pronouns,
                    ImageId = imageId,
                    ModifiedAtUTC = now,
                    ModifiedBy = userId,
                };

            }

            return (userDto, imageDto);
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
