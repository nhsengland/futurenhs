using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Interfaces.Providers
{
    public interface ISmtpClient
    {
        Task SendMailAsync(MailMessage message, CancellationToken cancellationToken);
    }
}
