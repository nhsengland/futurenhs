namespace MvcForum.Core.Services
{
    using MvcForum.Core.Interfaces.Services;
    using SendGrid;
    using SendGrid.Helpers.Errors.Model;
    using SendGrid.Helpers.Mail;
    using System;
    using System.Configuration;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Service to send mail messages via SendGrid.
    /// </summary>
    public class SendGridEmailService : ISendEmailService
    {
        private readonly ISendGridClient _client;

        private readonly ILoggingService _logger;

        public SendGridEmailService(ILoggingService logger, ISendGridClient client)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Method to send an email via SendGrid.
        /// </summary>
        /// <param name="fromEmailAddress">From <see cref="MailAddress"/>.</param>
        /// <param name="recipientEmailAddress">To <see cref="MailAddress"/>.</param>
        /// <param name="subject">The subject of the mail message.</param>
        /// <param name="content">The mail content.</param>
        /// <param name="cancellationToken">Cancellation token to cancel current request.</param>
        /// <returns></returns>
        public async Task<bool> SendAsync(MailAddress fromEmailAddress, MailAddress recipientEmailAddress, string subject, string content, CancellationToken cancellationToken)
        {
            if (fromEmailAddress is null)
            {
                throw new ArgumentNullException(nameof(fromEmailAddress));
            }

            if (recipientEmailAddress is null)
            {
                throw new ArgumentNullException(nameof(recipientEmailAddress));
            }

            var message = new SendGridMessage
            {
                From = new EmailAddress(fromEmailAddress.Address),
                Subject = subject,
                HtmlContent = content,
            };

            message.AddTo(new EmailAddress(recipientEmailAddress.Address));

            var response = await _client.SendEmailAsync(message, cancellationToken);

            return response.IsSuccessStatusCode;
        }
    }
}
