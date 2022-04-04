using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Providers.Interfaces;
using Notify.Client;
using Notify.Exceptions;
using System.Net.Mail;

namespace FutureNHS.Api.Providers
{
    public class GovNotifyProvider : INotificationProvider
    {
        private readonly ILogger<GovNotifyProvider> _logger;
        private readonly string _apiKey;

        public GovNotifyProvider(string apiKey, ILogger<GovNotifyProvider> logger)
        {
            if(string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiKey = apiKey;
        }

        public async Task SendEmailAsync(MailAddress emailAddress, string templateId, Dictionary<string,dynamic>? parameters = null, string? clientReference = null, string? replyToId = null)
        {
            try
            {
                var notificationClient = new NotificationClient(_apiKey);
                var result = await notificationClient.SendEmailAsync(emailAddress.Address, templateId, parameters, clientReference, replyToId);
            }
            catch (NotifyClientException ex)
            {
                _logger.LogError(ex,$"Error: SendEmail");
                throw new DependencyFailedException($"Error: Failed to send email");
            }

        }
    }
}
