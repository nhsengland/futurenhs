using MvcForum.Core.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Interfaces.Factories
{
    public interface ISmtpClientFactory
    {
        ISmtpClient CreateInstance();
    }
}
