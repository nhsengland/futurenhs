using Moq;
using MvcForum.Core.Interfaces.Factories;
using MvcForum.Core.Interfaces.Providers;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Tests.Services
{
    public class SmtpEmailServiceSpec
    {
        private Mock<ISmtpClient> _client;

        private Mock<ISmtpClientFactory> _clientFactory;


        [SetUp]
        public void Setup()
        {
            _client = new Mock<ISmtpClient>();
            _clientFactory = new Mock<ISmtpClientFactory>();
        }


        [Test]
        public void CanSendEmailAsync()
        {
            _client.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _clientFactory.Setup(x => x.CreateInstance()).Returns(_client.Object);

            var smtpService = new SmtpEmailService(new LoggingService(), _clientFactory.Object);
            var result = smtpService.SendAsync(new MailAddress("email@example.com"), new MailAddress("email2@example.com"), "subject", "content", CancellationToken.None).Result;

            Assert.True(result);
        }

        [Test]
        public void ThrowsSmtpExceptionOnFailedSend()
        {
            _client.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>())).Throws<SmtpException>();
            _clientFactory.Setup(x => x.CreateInstance()).Returns(_client.Object);

            var smtpService = new SmtpEmailService(new LoggingService(), _clientFactory.Object);
            
            Assert.ThrowsAsync<SmtpException>(async () =>
            {
                await smtpService.SendAsync(new MailAddress("email@example.com"), new MailAddress("email2@example.com"), "subject", "content", CancellationToken.None);
            });
        }

        [Test]
        public void ThrowsInvalidMailAddress()
        {
            _client.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _clientFactory.Setup(x => x.CreateInstance()).Returns(_client.Object);

            var smtpService = new SmtpEmailService(new LoggingService(), _clientFactory.Object);
            Assert.ThrowsAsync<ArgumentException>(async () => { 
                await smtpService.SendAsync(new MailAddress(""), new MailAddress("email2@example.com"), "subject", "content", CancellationToken.None); 
            });
        }

        [Test]
        public void ThrowsOnNullFromEmailAddress()
        {
            _client.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _clientFactory.Setup(x => x.CreateInstance()).Returns(_client.Object);

            var smtpService = new SmtpEmailService(new LoggingService(), _clientFactory.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await smtpService.SendAsync(null, new MailAddress("email2@example.com"), "subject", "content", CancellationToken.None);
            });
        }

        [Test]
        public void ThrowsOnNullRecipientEmailAddress()
        {
            _client.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _clientFactory.Setup(x => x.CreateInstance()).Returns(_client.Object);

            var smtpService = new SmtpEmailService(new LoggingService(), _clientFactory.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await smtpService.SendAsync(new MailAddress("email2@example.com"), null, "subject", "content", CancellationToken.None);
            });
        }
    }
}


