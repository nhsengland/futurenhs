namespace MvcForum.Core.Providers.Mail
{
    using MvcForum.Core.Interfaces.Providers;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    public class SmtpClientWrapper : SmtpClient, ISmtpClient
    {
        public SmtpClientWrapper(string host, int port) : base(host, port)
        {
        }

        public Task SendMailAsync(MailMessage message, CancellationToken cancellationToken)
        {
            return Task.Run(() => { base.SendMailAsync(message); }, cancellationToken);
        }
    }
}
