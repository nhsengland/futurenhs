using MvcForum.Core.Interfaces.Factories;
using MvcForum.Core.Interfaces.Providers;
using MvcForum.Core.Providers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Factories
{
    public class SmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpClient CreateInstance()
        {
            return new SmtpClientWrapper(
                ConfigurationManager.AppSettings["Email_SmtpHost"],
                Int32.Parse(ConfigurationManager.AppSettings["Email_SmtpPort"]))
            {
                Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["Email_SmtpUsername"],
                    ConfigurationManager.AppSettings["Email_SmtpPassword"])
            };
        }
    }
}
