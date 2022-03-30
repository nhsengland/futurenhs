using System.Net.Mail;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(MailAddress emailAddress, string templateId);
    }
}
