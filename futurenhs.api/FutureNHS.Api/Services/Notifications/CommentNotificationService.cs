using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Notifications.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace FutureNHS.Api.Services.Notifications
{
    public sealed class CommentNotificationService : ICommentNotificationService
    {
        private readonly string _fqdn;
        private readonly ILogger<CommentNotificationService> _logger;
        private readonly IEmailService _emailService;
        private readonly IDiscussionDataProvider _discussionDataProvider;
        private readonly GovNotifyConfiguration _govNotifyConfiguration;

        public CommentNotificationService(ILogger<CommentNotificationService> logger,
            IEmailService emailService,
            IOptionsSnapshot<GovNotifyConfiguration> notifyConfig,
            IOptionsSnapshot<ApplicationGateway> gatewayConfig,
            IDiscussionDataProvider discussionDataProvider)
        {
            _logger = logger;
            _emailService = emailService;
            _fqdn = gatewayConfig.Value.FQDN;
            _discussionDataProvider = discussionDataProvider;
            _govNotifyConfiguration = notifyConfig.Value;
        }

        public async Task SendNotificationToDiscussionCreatorAsync(Guid posterId, Guid discussionId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == discussionId) throw new ArgumentOutOfRangeException(nameof(discussionId));
            if (Guid.Empty == posterId) throw new ArgumentOutOfRangeException(nameof(posterId));

            var discussionCreatorDetails = await _discussionDataProvider.GetDiscussionCreatorDetailsAsync(discussionId, cancellationToken);
            var discussionLink = $"{_fqdn}/groups/{discussionCreatorDetails.GroupSlug}/forum/{discussionCreatorDetails.DiscussionId}";

            if (discussionCreatorDetails.CreatedById == posterId)
            {
                return;
            }

            MailAddress creatorEmailAddress;
            try
            {
                creatorEmailAddress = new MailAddress(discussionCreatorDetails.CreatedByEmail);
            }
            catch (Exception)
            {
                _logger.LogError("Discussion creator's email address({emailAddress}) is not in a correct format", discussionCreatorDetails.CreatedByEmail);
                throw new FormatException($"Email is not in a valid format");
            }

            var personalisation = new Dictionary<string, dynamic>
            {
                {"discussion_link", discussionLink}
            };

            await _emailService.SendEmailAsync(creatorEmailAddress, _govNotifyConfiguration.CommentOnDiscussionEmailTemplateId, personalisation);
        }
    }
}
