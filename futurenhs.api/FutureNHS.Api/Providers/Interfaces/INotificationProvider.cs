using System.Net.Mail;

namespace FutureNHS.Api.Providers.Interfaces
{
    public interface INotificationProvider
    {
        Task SendEmailAsync(MailAddress emailAddress, string templateId, Dictionary<string, dynamic>? parameters = null,
            string? clientReference = null, string? replyToId = null);
    }
}
