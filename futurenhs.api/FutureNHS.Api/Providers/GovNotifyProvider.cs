using System.Net.Mail;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Providers.Interfaces;
using Notify.Client;
using Notify.Exceptions;
using Notify.Interfaces;

namespace FutureNHS.Api.Providers
{
    public class GovNotifyProvider : INotificationProvider
    {
        private readonly ILogger<GovNotifyProvider> _logger;
        private readonly INotificationClient _notificationClient;
        public GovNotifyProvider(string apiKey, ILogger<GovNotifyProvider> logger)
        {
            if(string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationClient = new NotificationClient(apiKey);

        }

        public void SendEmail(MailAddress emailAddress, string templateId, Dictionary<string,dynamic>? parameters = null, string? clientReference = null, string? replyToId = null)
        {
            try
            {
                var result = _notificationClient.SendEmail(emailAddress.Address, templateId, parameters, clientReference, replyToId);
            }
            catch (NotifyClientException ex)
            {
                _logger.LogError(ex,$"Error: SendEmail");
                throw new DependencyFailedException($"Error: Failed to send email");
            }

        }
    }
}
