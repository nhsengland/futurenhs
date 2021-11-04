namespace MvcForum.Core.Interfaces.Services
{
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IEmailProvider"/>.
    /// </summary>
    public interface ISendEmailService
    {
        /// <summary>
        /// Method to send an email.
        /// </summary>
        /// <param name="fromEmailAddress"></param>
        /// <param name="recipientEmailAddress"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        Task<bool> SendAsync(MailAddress fromEmailAddress, MailAddress recipientEmailAddress, string subject, string content, CancellationToken cancellationToken);
    }
}
