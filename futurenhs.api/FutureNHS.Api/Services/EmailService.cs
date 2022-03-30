using System.Net.Mail;
using FutureNHS.Api.Providers.Interfaces;
using FutureNHS.Api.Services.Interfaces;

namespace FutureNHS.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly INotificationProvider _notificationProvider;
        private readonly ILogger<EmailService> _logger;

        public EmailService(INotificationProvider notificationProvider, ILogger<EmailService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationProvider = notificationProvider ?? throw new ArgumentNullException(nameof(_notificationProvider));
        }

        public void SendEmail(MailAddress emailAddress, string templateId)
        {
            _notificationProvider.SendEmail(emailAddress, templateId);
        }

    }
}
