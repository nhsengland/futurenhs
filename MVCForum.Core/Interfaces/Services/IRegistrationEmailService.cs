using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Interfaces.Services
{
    /// <summary>
    /// Defines the <see cref="IRegistrationEmailService"/> interface.
    /// </summary>
    public interface IRegistrationEmailService
    {
        /// <summary>
        /// Defines method to send platform invitation.
        /// </summary>
        /// <param name="recipientEmailAddress">The recipient of the invitation.</param>
        Task<bool> SendInvitationAsync(MailAddress recipientEmailAddress, CancellationToken cancellationToken);
    }
}
