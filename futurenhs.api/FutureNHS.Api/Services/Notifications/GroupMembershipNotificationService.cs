using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Notifications.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace FutureNHS.Api.Services.Notifications
{
    public sealed class GroupMemberNotificationService : IGroupMemberNotificationService
    {
        private readonly ILogger<GroupMemberNotificationService> _logger;
        private readonly IEmailService _emailService;
        private readonly GovNotifyConfiguration _govNotifyConfiguration;
        private readonly IUserDataProvider _userDataProvider;

        public GroupMemberNotificationService(ILogger<GroupMemberNotificationService> logger,
            IEmailService emailService,
            IOptionsSnapshot<GovNotifyConfiguration> notifyConfig, 
            IUserDataProvider userDataProvider)
        {
            _logger = logger;
            _emailService = emailService;
            _govNotifyConfiguration = notifyConfig.Value;
            _userDataProvider = userDataProvider;
        }

        public async Task SendAcceptNotificationToMemberAsync(Guid membershipUserId, string groupName, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (string.IsNullOrEmpty(groupName)) throw new ArgumentOutOfRangeException(nameof(groupName));

            var memberDetails = await _userDataProvider.GetMemberAsync(membershipUserId, cancellationToken);

            if (memberDetails is null)
            {
                _logger.LogError("Could not find user for Id:{MembershipUserId}", membershipUserId);
                throw new NullReferenceException(nameof(memberDetails));
            }
            
            MailAddress memberEmailAddress;
            try
            {
                memberEmailAddress = new MailAddress(memberDetails.Email);
            }
            catch (Exception)
            {
                _logger.LogError("User's email address({emailAddress}) is not in a correct format", memberDetails.Email);
                throw new FormatException($"Email is not in a valid format");
            }

            var personalisation = new Dictionary<string, dynamic>
            {
                {"group_name", groupName}
            };

            await _emailService.SendEmailAsync(memberEmailAddress, _govNotifyConfiguration.GroupMemberRequestAcceptedEmailTemplateId, personalisation);
        }

        public async Task SendRejectNotificationToMemberAsync(Guid membershipUserId, string groupName, CancellationToken cancellationToken)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (string.IsNullOrEmpty(groupName)) throw new ArgumentOutOfRangeException(nameof(groupName));

            var memberDetails = await _userDataProvider.GetMemberAsync(membershipUserId, cancellationToken);

            if (memberDetails is null)
            {
                _logger.LogError("Could not find user for Id:{MembershipUserId}", membershipUserId);
                throw new NullReferenceException(nameof(memberDetails));
            }

            MailAddress memberEmailAddress;
            try
            {
                memberEmailAddress = new MailAddress(memberDetails.Email);
            }
            catch (Exception)
            {
                _logger.LogError("User's email address({emailAddress}) is not in a correct format", memberDetails.Email);
                throw new FormatException($"Email is not in a valid format");
            }

            var personalisation = new Dictionary<string, dynamic>
            {
                {"group_name", groupName}
            };

            await _emailService.SendEmailAsync(memberEmailAddress, _govNotifyConfiguration.GroupMemberRequestRejectedEmailTemplateId, personalisation);
        }
    }
}