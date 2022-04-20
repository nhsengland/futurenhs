using System.Net.Mail;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Services.Interfaces;
using System.Security;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Services
{
    public class UserService : IUserService
    {
        private const string ListMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/list";
        private const string AddMembersRole = $"https://schema.collaborate.future.nhs.uk/members/v1/add";
        private readonly string _fqdn;
        private readonly ILogger<UserService> _logger;
        private readonly IUserAdminDataProvider _userAdminDataProvider;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;
        private readonly IUserCommand _userCommand;
        private readonly IEmailService _emailService;

        // Notification template Ids
        private readonly string _registrationEmailId;

        public UserService(ILogger<UserService> logger, ISystemClock systemClock, IPermissionsService permissionsService, IUserAdminDataProvider userAdminDataProvider, IUserCommand userCommand, IEmailService emailService, IOptionsSnapshot<GovNotifyConfiguration> notifyConfig, IOptionsSnapshot<ApplicationGateway> gatewayConfig)
        {
            _permissionsService = permissionsService;
            _userAdminDataProvider = userAdminDataProvider;
            _systemClock = systemClock;
            _logger = logger;
            _userCommand = userCommand;
            _emailService = emailService;
            _fqdn = gatewayConfig.Value.FQDN;

            // Notification template Ids
            _registrationEmailId = notifyConfig.Value.RegistrationEmailTemplateId;
        }


        public async Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchUsers(Guid userId, string term, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }


            if (term.Length is < SearchSettings.TermMinimum or > SearchSettings.TermMaximum)
            {
                throw new ArgumentOutOfRangeException(nameof(term));
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, ListMembersRole, cancellationToken);
            
            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: Search Users - User:{0} does not have access to perform admin actions", userId.ToString());
                throw new SecurityException($"Error: User does not have access");
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await _userCommand.SearchUsers(term, offset, limit,sort, cancellationToken);
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

        private string CreateRegistrationLink()
        {
            var registrationLink = $"{_fqdn}/members/register";
            return registrationLink;
        }
    }
}
