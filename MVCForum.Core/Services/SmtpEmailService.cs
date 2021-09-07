namespace MvcForum.Core.Services
{
    using MvcForum.Core.Interfaces.Factories;
    using MvcForum.Core.Interfaces.Services;
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Service to send mail messages via Smtp.
    /// </summary>
    public class SmtpEmailService : ISendEmailService
    {
        private readonly ISmtpClientFactory _clientFactory;

        private readonly ILoggingService _logger;

        public SmtpEmailService(ILoggingService logger, ISmtpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Method to send an email via SMTP
        /// </summary>
        /// <param name="fromEmailAddress">From <see cref="MailAddress"/>.</param>
        /// <param name="recipientEmailAddress">To <see cref="MailAddress"/>.</param>
        /// <param name="subject">The subject of the mail message.</param>
        /// <param name="content">The content of the mail message.</param>
        /// <param name="cancellationToken">Current cancellation token.</param>
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

            MailMessage message = new MailMessage(fromEmailAddress.Address, recipientEmailAddress.Address)
            {
                Subject = subject,
                Body = content
            };

            var client = _clientFactory.CreateInstance();
            try
            {
                await client.SendMailAsync(message, cancellationToken);
            } catch (SmtpException ex)
            {
                _logger.Error(ex);
                throw;
            }

            return true;
        }
    }
}
