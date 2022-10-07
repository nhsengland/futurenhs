using System.Data;
using System.Net.Mail;
using System.Security;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Registration;
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

        private readonly string _fqdn;
        private readonly ILogger<AdminUserService> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;
        private readonly IUserCommand _userCommand;
        private readonly IEmailService _emailService;
        private readonly IGroupCommand _groupCommand;
        private readonly IRegistrationDataProvider _registrationDataProvider;
        private readonly IUserService _userService;
        // Notification template Ids
        private readonly string _registrationEmailId;
        private readonly string _defaultRole;


        public RegistrationService(ILogger<AdminUserService> logger,
            ISystemClock systemClock,
            IPermissionsService permissionsService, 
            IUserCommand userCommand,
            IEmailService emailService,
            IUserService userService,
            IGroupCommand groupCommand,
            IRegistrationDataProvider registrationDataProvider,
            IOptionsSnapshot<GovNotifyConfiguration> notifyConfig,
            IOptionsSnapshot<ApplicationGateway> gatewayConfig,
            IOptionsMonitor<DefaultSettings> defaultSettings)
        {
            _groupCommand = groupCommand;
            _permissionsService = permissionsService;
            _registrationDataProvider = registrationDataProvider;
            _systemClock = systemClock;
            _logger = logger;
            _userService = userService;
            _userCommand = userCommand;
            _emailService = emailService;
            _fqdn = gatewayConfig.Value.FQDN;

            // Notification template Ids
            _registrationEmailId = notifyConfig.Value.RegistrationEmailTemplateId;
            _defaultRole = defaultSettings.CurrentValue.DefaultRole ?? throw new ArgumentOutOfRangeException(nameof(defaultSettings.CurrentValue.DefaultRole));

        }
        
        public async Task InviteMemberToGroupAndPlatformAsync(Guid userId, string? groupSlug, string email, CancellationToken cancellationToken)
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

            var groupId = string.IsNullOrEmpty(groupSlug)
                ? null
                : await _groupCommand.GetGroupIdForSlugAsync(groupSlug, cancellationToken);

            var userInvite = new GroupInviteDto
            {
                EmailAddress = emailAddress.Address.ToLowerInvariant(),
                GroupId = groupId,
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                CreatedBy = userId

            };

            var userInviteId = await _userCommand.CreateInviteUserAsync(userInvite, cancellationToken);
            var registrationLink = CreateRegistrationLink(userInviteId);
            var personalisation = new Dictionary<string, dynamic>
            {
                {"registration_link", registrationLink}
            };
            
            await _emailService.SendEmailAsync(emailAddress, _registrationEmailId, personalisation);
        }
        public async Task InviteMemberToPlatformAsync(Guid userId, string? groupSlug, string email, CancellationToken cancellationToken)
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

            var groupId = string.IsNullOrEmpty(groupSlug)
                ? null
                : await _groupCommand.GetGroupIdForSlugAsync(groupSlug, cancellationToken);

            var userInvite = new GroupInviteDto
            {
                EmailAddress = emailAddress.Address.ToLowerInvariant(),
                GroupId = groupId,
                CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                CreatedBy = userId

            };

            var userInviteId = await _userCommand.CreateInviteUserAsync(userInvite, cancellationToken);
            var registrationLink = CreateRegistrationLink(userInviteId);
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

            // TODO Work for determining if domain is on auto approve list
            var emailAddress = new MailAddress(registrationRequest.Email);
            var domain = emailAddress.Host;

            if (await _userService.IsMemberInvitedAsync(registrationRequest.Email, cancellationToken))
            {
                // todo validate user

                var member = new MemberDto
                {
                    FirstName = registrationRequest.FirstName,
                    Surname = registrationRequest.LastName,
                    Email = registrationRequest.Email,
                    CreatedAtUTC = _systemClock.UtcNow.UtcDateTime,
                    AgreedToTerms = registrationRequest.Agreed
                };
                try
                {
                    return await _userCommand.RegisterUserAsync(member, registrationRequest.Subject, registrationRequest.Issuer, _defaultRole, cancellationToken);
                }
                catch (DBConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Error: Error registering new user");
                    throw;
                }
            }
            return null;
        }


        public async Task<InviteDetails> GetRegistrationInviteDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            var invite = await _registrationDataProvider.GetRegistrationInviteAsync(id, cancellationToken);
            return invite;
        }

        private string CreateRegistrationLink(Guid userInviteId)
        {
            var registrationLink = $"{_fqdn}/auth/invited?id={userInviteId}";
            return registrationLink;
        }
    }
}
