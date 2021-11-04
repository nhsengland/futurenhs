namespace MvcForum.Core.Services
{
    using MvcForum.Core.Interfaces.Providers;
    using MvcForum.Core.Interfaces.Services;
    using System;
    using System.Configuration;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Routing;

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
            var registrationLink = new UriBuilder(ConfigurationManager.AppSettings["AzurePlatform:ApplicationGateway:FQDN"]) 
            { 
                Path = "/members/register" 
            };

            var bodyText = new StringBuilder(_localizationService.GetResourceString("Email.Invite.Body"));
            bodyText
                .Append("<br><br><a href='")
                .Append(registrationLink.Uri.ToString())
                .Append("'>")
                .Append(_localizationService.GetResourceString("Email.Invite.Link"))
                .Append("</a>");
            
            return _sendEmailService.SendAsync(
                new MailAddress(_configurationProvider.SmtpFrom), 
                recipientEmailAddress, 
                _localizationService.GetResourceString("Email.Invite.Subject"), 
                bodyText.ToString(),
                cancellationToken
            );
        }
    }
}
