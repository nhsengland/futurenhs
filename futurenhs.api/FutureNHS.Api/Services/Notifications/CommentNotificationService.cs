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
        private readonly ICommentsDataProvider _commentsDataProvider;
        private readonly GovNotifyConfiguration _govNotifyConfiguration;

        public CommentNotificationService(ILogger<CommentNotificationService> logger,
            IEmailService emailService,
            IOptionsSnapshot<GovNotifyConfiguration> notifyConfig,
            IOptionsSnapshot<ApplicationGateway> gatewayConfig,
            IDiscussionDataProvider discussionDataProvider,
            ICommentsDataProvider commentsDataProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _fqdn = gatewayConfig.Value.FQDN ?? throw new ArgumentNullException(nameof(gatewayConfig.Value.FQDN));
            _discussionDataProvider = discussionDataProvider ?? throw new ArgumentNullException(nameof(discussionDataProvider));
            _govNotifyConfiguration = notifyConfig.Value ?? throw new ArgumentNullException(nameof(notifyConfig.Value));
            _commentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
        }

        public async Task SendNotificationToDiscussionCreatorAsync(Guid posterId, Guid discussionId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == discussionId) throw new ArgumentOutOfRangeException(nameof(discussionId));
            if (Guid.Empty == posterId) throw new ArgumentOutOfRangeException(nameof(posterId));

            var discussionCreatorDetails = await _discussionDataProvider.GetDiscussionCreatorDetailsAsync(discussionId, cancellationToken);
            var discussionLink = $"{_fqdn}/groups/{discussionCreatorDetails.GroupSlug}/forum/{discussionCreatorDetails.DiscussionId}";

            var posterIsOwner = discussionCreatorDetails.CreatedById == posterId;
            if (posterIsOwner)
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

        public async Task SendNotificationToCommentCreatorAsync(Guid posterId, Guid commentId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == commentId) throw new ArgumentOutOfRangeException(nameof(commentId));
            if (Guid.Empty == posterId) throw new ArgumentOutOfRangeException(nameof(posterId));

            var commentCreatorDetails = await _commentsDataProvider.GetCommentCreatorDetailsAsync(commentId, cancellationToken);
            var commentLink = $"{_fqdn}/groups/{commentCreatorDetails.GroupSlug}/forum/{commentCreatorDetails.DiscussionId}";

            var posterIsOwner = commentCreatorDetails.CreatedById == posterId;
            if (posterIsOwner)
            {
                return;
            }

            MailAddress creatorEmailAddress;
            try
            {
                creatorEmailAddress = new MailAddress(commentCreatorDetails.CreatedByEmail);
            }
            catch (Exception)
            {
                _logger.LogError("Comment creator's email address({emailAddress}) is not in a correct format", commentCreatorDetails.CreatedByEmail);
                throw new FormatException($"Email is not in a valid format");
            }

            var personalisation = new Dictionary<string, dynamic>
            {
                {"comment_link", commentLink}
            };

            await _emailService.SendEmailAsync(creatorEmailAddress, _govNotifyConfiguration.ResponseToCommentEmailTemplateId, personalisation);
        }
    }
}
