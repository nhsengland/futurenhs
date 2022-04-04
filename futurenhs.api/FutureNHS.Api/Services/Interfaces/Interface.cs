using System.Net.Mail;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailAddress emailAddress, string templateId, Dictionary<string, dynamic>? parameters = null);
    }
}
