namespace MvcForum.Core.Services
{
    using MvcForum.Core.Interfaces.Providers;
    using MvcForum.Core.Interfaces.Services;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    public class RegistrationEmailService : IRegistrationEmailService
    {
        private readonly IConfigurationProvider _configurationProvider;

        private readonly ILocalizationService _localizationService;

        private readonly ISendEmailService _sendEmailService;

        /// <summary>
        /// Constructs a new instance of the <see cref="RegistrationEmailService"/>.
        /// </summary>
        /// <param name="localizationService">Instance of the <see cref="ILocalizationService"/>.</param>
        public RegistrationEmailService(IConfigurationProvider configurationProvider, ILocalizationService localizationService, ISendEmailService sendEmailService)
        {
            _configurationProvider = configurationProvider;
            _localizationService = localizationService;
            _sendEmailService = sendEmailService;
        }

        /// <summary>
        /// Method to send platform invitation email.
        /// </summary>
        /// <param name="recipientEmail">The email address of the recipient.</param>
        public Task<bool> SendInvitationAsync(MailAddress recipientEmailAddress, CancellationToken cancellationToken)
        {
            return _sendEmailService.SendAsync(
                new MailAddress(_configurationProvider.SmtpFrom), 
                recipientEmailAddress, 
                _localizationService.GetResourceString("Email.Invite.Subject"), 
                _localizationService.GetResourceString("Email.Invite.Body"),
                cancellationToken
            );
        }
    }
}
