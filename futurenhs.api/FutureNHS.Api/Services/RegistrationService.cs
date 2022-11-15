using System.Data;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Domain;
using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Domain.Request;
using FutureNHS.Api.Models.Identity.Request;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member.Request;
using FutureNHS.Api.Services.Admin;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Services
{
    public sealed class RegistrationService : IRegistrationService
    {
        private const string AddMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/add";
        private const string ViewDomainsRole = $"https://schema.collaborate.future.nhs.uk/domain/v1/view";
        private const string AddDomainRole = $"https://schema.collaborate.future.nhs.uk/domain/v1/add";
        private const string UpdateDomainRole = $"https://schema.collaborate.future.nhs.uk/domain/v1/edit";
        private const string DeleteDomainRole = $"https://schema.collaborate.future.nhs.uk/domain/v1/delete";

        private readonly string _fqdn;
        private readonly ILogger<AdminUserService> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;
        private readonly IUserCommand _userCommand;
        private readonly IEmailService _emailService;
        private readonly IGroupCommand _groupCommand;
        private readonly IDomainCommand _domainCommand;
        private readonly IDomainDataProvider _domainDataProvider;
        private readonly IRegistrationDataProvider _registrationDataProvider;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        // Notification template Ids
        private readonly string _registrationEmailId;
        private readonly string _defaultRole;
        private readonly string _groupRegistrationEmailId;
        private readonly string _groupInviteEmailId;


        public RegistrationService(ILogger<AdminUserService> logger,
            ISystemClock systemClock,
            IPermissionsService permissionsService, 
            IUserCommand userCommand,
            IEmailService emailService,
            IUserService userService,
            IGroupCommand groupCommand,
            IGroupService groupService,
            IRegistrationDataProvider registrationDataProvider,
            IOptionsSnapshot<GovNotifyConfiguration> notifyConfig,
            IOptionsSnapshot<ApplicationGateway> gatewayConfig,
            IDomainCommand domainCommand,
            IDomainDataProvider domainDataProvider,
            IOptionsMonitor<DefaultSettings> defaultSettings)
        {
            _groupCommand = groupCommand;
            _groupService = groupService;
            _permissionsService = permissionsService;
            _registrationDataProvider = registrationDataProvider;
            _systemClock = systemClock;
            _logger = logger;
            _userService = userService;
            _userCommand = userCommand;
            _emailService = emailService;
            _domainCommand = domainCommand;
            _domainDataProvider = domainDataProvider;
            _fqdn = gatewayConfig.Value.FQDN;

            // Notification template Ids
            _registrationEmailId = notifyConfig.Value.RegistrationEmailTemplateId;
            _groupRegistrationEmailId = notifyConfig.Value.GroupRegistrationEmailTemplateId;
            _groupInviteEmailId = notifyConfig.Value.GroupInviteEmailTemplateId;
            _defaultRole = defaultSettings.CurrentValue.DefaultRole ?? throw new ArgumentOutOfRangeException(nameof(defaultSettings.CurrentValue.DefaultRole));

        }

        private async Task<Guid> CreatePlatformInviteAsync(Guid userId, MailAddress emailAddress, Guid? groupId, CancellationToken cancellationToken)
        {
            var domain = emailAddress.Host;
            var domainIsAllowed = await _domainDataProvider.IsDomainApprovedAsync(domain, cancellationToken);
            if (!domainIsAllowed)
            {
                throw new InvalidOperationException("The email address cannot be invited");
            }

            var userInvite = new PlatformInviteDto
            {
                EmailAddress = emailAddress.Address.ToLowerInvariant(),
                GroupId = groupId,
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                CreatedBy = userId
            };

            return await _userCommand.CreateInviteUserAsync(userInvite, cancellationToken);
        }
        
        private async Task<Guid> CreateGroupInviteAsync(Guid userId, Guid invitedUserId, Guid groupId, CancellationToken cancellationToken)
        {
            var userInvite = new GroupInviteDto
            {
                MembershipUser_Id = invitedUserId,
                GroupId = groupId,
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                CreatedBy = userId,
                ExpiresAtUTC = null
            };

            return await _userCommand.CreateInviteGroupUserAsync(userInvite, cancellationToken);
        }
        public async Task InviteMemberToGroupAndPlatformAsync(Guid userId, string groupSlug, string email,
            CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(groupSlug)) throw new ArgumentOutOfRangeException(nameof(groupSlug));


            var userCanPerformAction =
                await _permissionsService.UserCanPerformActionAsync(userId, AddMembersRole, cancellationToken);
            if (!userCanPerformAction)
            {
                _logger.LogError(
                    $"Error: InviteMemberToGroupAndPlatformAsync - User:{0} does not have access to perform admin actions",
                    userId);
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

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(groupSlug, cancellationToken);
            
            var userIsOnPlatform = await _userCommand.GetMemberByEmailAsync(email, cancellationToken);
            if (userIsOnPlatform is null)
            {
                var userInviteId = await _userService.GetInviteIdForEmailAsync(email, cancellationToken, groupId);
            
                if (userInviteId.HasValue is false)
                {
                    userInviteId = await CreatePlatformInviteAsync(userId, emailAddress, groupId, cancellationToken);
                }
                //TODO: Check user is on platform and add to group invites list
                var registrationLink = CreateRegistrationLink(userInviteId.Value);
                var inviter = await _userService.GetMemberAsync(userId, targetUserId: userId, cancellationToken);
                var groupName = await _groupService.GetGroupAsync(userId, groupSlug, cancellationToken);
                var personalisation = new Dictionary<string, dynamic>
                {
                    { "registration_link", registrationLink },
                    { "inviter", inviter.FullName },
                    { "group_name", groupName.Name }
                };

                await _emailService.SendEmailAsync(emailAddress, _groupRegistrationEmailId, personalisation);
            }

            else
            {
                if (groupId is null)
                {
                    throw new NotFoundException($"Group Id was not found.");
                }
                var memberDetails = await _userCommand.GetMemberByEmailAsync(email, cancellationToken);
                if (memberDetails is null)
                {
                    throw new NotFoundException($"Group Id was not found.");
                }
                var userGroupInviteId = await _userService.GetGroupInviteIdForMemberAsync(memberDetails.Id, groupId.Value, cancellationToken);
            
                if (userGroupInviteId.HasValue is false)
                {
                    userGroupInviteId = await CreateGroupInviteAsync(userId, memberDetails.Id, groupId.Value, cancellationToken);
                }

                var registrationLink = _fqdn;
                var inviter = await _userService.GetMemberAsync(userId, targetUserId: userId, cancellationToken);
                var groupName = await _groupService.GetGroupAsync(userId, groupSlug, cancellationToken);
                var personalisation = new Dictionary<string, dynamic>
                {
                    { "registration_link", registrationLink },
                    { "inviter", inviter.FullName },
                    { "group_name", groupName.Name }
                };
                await _emailService.SendEmailAsync(emailAddress, _groupInviteEmailId, personalisation);
            }
        }
        public async Task InviteMemberToPlatformAsync(Guid userId, string email, CancellationToken cancellationToken)
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

            var userInviteId = await _userService.GetInviteIdForEmailAsync(email, cancellationToken);
            
            if (userInviteId.HasValue is false)
            {
                userInviteId = await CreatePlatformInviteAsync(userId, emailAddress, null, cancellationToken);
            }
            var registrationLink = CreateRegistrationLink(userInviteId.Value);
            var personalisation = new Dictionary<string, dynamic>
            {
                {"registration_link", registrationLink}
            };
            
            await _emailService.SendEmailAsync(emailAddress, _registrationEmailId, personalisation);
        }


        public async Task<Guid?> RegisterMemberAsync(MemberRegistrationRequest registrationRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(registrationRequest.Subject)) throw new ArgumentNullException(nameof(registrationRequest.Subject));
            if (string.IsNullOrEmpty(registrationRequest.Email)) throw new ArgumentNullException(nameof(registrationRequest.Email));
            if (string.IsNullOrEmpty(registrationRequest.Issuer)) throw new ArgumentNullException(nameof(registrationRequest.Issuer));
            
            MailAddress emailAddress;
            try
            {
                emailAddress = new MailAddress(registrationRequest.Email);
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException($"Email is not in a valid format");
            }
            
            var domain = emailAddress.Host;
            var memberCanRegisterConditions = await Task.WhenAll(new[]
            {
                _domainDataProvider.IsDomainApprovedAsync(domain, cancellationToken),
                _userService.IsMemberInvitedAsync(registrationRequest.Email, cancellationToken)
            });
            var memberCanRegister = memberCanRegisterConditions.All(x => x);
            if (memberCanRegister)
            {
                var member = new MemberDto
                {
                    FirstName = registrationRequest.FirstName,
                    Surname = registrationRequest.LastName,
                    Email = emailAddress.Address,
                    CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                    AgreedToTerms = registrationRequest.Agreed
                };
                try
                {
                    var user = await _userCommand.RegisterUserAsync(member, registrationRequest.Subject, registrationRequest.Issuer, _defaultRole, cancellationToken);
                    await _userCommand.RedeemPlatformInviteAsync(user, registrationRequest.Email, cancellationToken);
                    return user;
                }
                catch (DBConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Error: Error registering new user");
                    throw;
                }
            }
            return null;
        }

        public async Task<MemberInfoResponse> MapMemberToIdentityAsync(MemberIdentityRequest memberIdentityRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(memberIdentityRequest.SubjectId)) throw new ArgumentOutOfRangeException(nameof(memberIdentityRequest.SubjectId));
            if (string.IsNullOrWhiteSpace(memberIdentityRequest.EmailAddress)) throw new ArgumentOutOfRangeException(nameof(memberIdentityRequest.EmailAddress));
            if (string.IsNullOrWhiteSpace(memberIdentityRequest.Issuer)) throw new ArgumentOutOfRangeException(nameof(memberIdentityRequest.Issuer));

            var memberInfo = await _userCommand.GetMemberInfoAsync(memberIdentityRequest.SubjectId, cancellationToken);
            if (memberInfo is not null)
            {
                throw new PreconditionFailedExeption("Precondition Failed: User already mapped to this identity");
            }

            var memberDetailsResponse = await _userCommand.GetMemberByEmailAsync(memberIdentityRequest.EmailAddress, cancellationToken); ;
            if (memberDetailsResponse is not null)
            {
                await _userCommand.MapIdentityToExistingUserAsync(memberDetailsResponse.Id, memberIdentityRequest.SubjectId, memberIdentityRequest.Issuer, cancellationToken);
                return await _userCommand.GetMemberInfoAsync(memberIdentityRequest.SubjectId, cancellationToken);
            }
            throw new NotFoundException("Could not find a user with this email to map the identity to");
        }

        public async Task<InviteDetails> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            var invite = await _registrationDataProvider.GetRegistrationInviteAsync(id, cancellationToken);
            return invite;
        }

        public async Task DeleteDomainAsync(Guid userId, Guid domainId, byte[] rowVersion, CancellationToken cancellationToken)
        {
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, DeleteDomainRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: DeleteDomainAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            try
            {         
                var domainDto = new DomainDto
                {
                    Id = domainId,
                    RowVersion = rowVersion
                };
                await _domainCommand.DeleteApprovedDomainAsync(domainDto, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: Error updating domain");
                throw;
            }
        }
        
        public async Task AddDomainAsync(Guid userId, RegisterDomainRequest domainRequest, CancellationToken cancellationToken)
        {
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, AddDomainRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: AddDomainAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            bool wasDomainDeleted;

            try
            {
                wasDomainDeleted =
                    await _domainDataProvider.IsDomainDeletedAsync(domainRequest.EmailDomain, cancellationToken);
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error: Error checking delete status of new domain");
                throw;
            }

            if (wasDomainDeleted)
            {
                try
                {
                    var domain = await _domainCommand.GetDeletedDomainAsync(domainRequest.EmailDomain, cancellationToken);
                    var domainDto = new DomainDto
                    {
                        Id = domain.Id,
                        RowVersion = domain.RowVersion
                    };
                    await _domainCommand.RestoreApprovedDomainAsync(domainDto, cancellationToken);
                }
                catch (DBConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Error: Error restoring deleted domain");
                    throw;
                }
            }
            else
            {
                try
                {
                    var domainDto = new DomainDto
                    {
                        EmailDomain = domainRequest.EmailDomain
                    };
                    await _domainCommand.CreateApprovedDomainAsync(domainDto, cancellationToken);
                }
                catch (DBConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Error: Error creating new domain");
                    throw;
                }
            }

        }

        public async Task<DomainDto> GetDomainAsync(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == id) throw new ArgumentOutOfRangeException(nameof(id));
            
            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, ViewDomainsRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetDomainAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _domainCommand.GetDomainAsync(id, cancellationToken);
        }

        public async Task<(uint, IEnumerable<ApprovedDomain>)> GetDomainsAsync(Guid userId, uint offset, uint limit, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, ViewDomainsRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: GetDomainsAsync - User:{0} does not have access to perform admin actions", userId);
                throw new SecurityException($"Error: User does not have access");
            }

            return await _domainCommand.GetDomainsAsync(offset, limit, cancellationToken);
        }

        private string CreateRegistrationLink(Guid userInviteId)
        {
            var registrationLink = $"{_fqdn}/auth/invited?id={userInviteId}";
            return registrationLink;
        }
    }
}
