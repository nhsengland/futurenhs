using Moq;
using MvcForum.Core.Interfaces.Services;
using NUnit.Framework;
using SendGrid;
using SendGrid.Helpers.Mail;
using MvcForum.Core.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using SendGrid.Helpers.Errors.Model;
using System;

namespace MvcForum.Core.Tests.Services
{
    public class SendGridEmailServiceSpec
    {
        private Mock<ILoggingService> _logger;
        private Mock<ISendGridClient> _client;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILoggingService>();
            _client = new Mock<ISendGridClient>();
        }

        [Test]
        public void CanSendEmailViaSendGrid()
        {
            _client.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None))
                .Returns(Task.FromResult<Response>(new Response(HttpStatusCode.OK, null, null)));

            var sendGridService = new SendGridEmailService(_logger.Object, _client.Object);

            var result = sendGridService.SendAsync(new MailAddress("test@address.com"), new MailAddress("test2@address.com"), "subject", "content", CancellationToken.None).Result;

            Assert.True(result);
        }

        [Test]
        public void ReturnsFalseOnFailedSend()
        {
            _client.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None))
                .Returns(Task.FromResult<Response>(new Response(HttpStatusCode.InternalServerError, null, null)));

            var sendGridService = new SendGridEmailService(_logger.Object, _client.Object);

            var result = sendGridService.SendAsync(new MailAddress("test@address.com"), new MailAddress("test2@address.com"), "subject", "content", CancellationToken.None).Result;

            Assert.False(result);
        }

        [Test]
        public void ThrowsInvalidMailAddress()
        {
            _client.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None))
                .Returns(Task.FromResult<Response>(new Response(HttpStatusCode.OK, null, null)));

            var sendGridService = new SendGridEmailService(_logger.Object, _client.Object);

            Assert.ThrowsAsync<ArgumentException>(async () => {
                await sendGridService.SendAsync(new MailAddress(""), new MailAddress("test2@address.com"), "subject", "content", CancellationToken.None);
            });

            Assert.ThrowsAsync<ArgumentException>(async () => {
                await sendGridService.SendAsync(new MailAddress("test2@address.com"), new MailAddress(""), "subject", "content", CancellationToken.None);
            });
        }

        [Test]
        public void ThrowsOnNullFromEmailAddress()
        {
            _client.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None))
                .Returns(Task.FromResult<Response>(new Response(HttpStatusCode.OK, null, null)));

            var sendGridService = new SendGridEmailService(_logger.Object, _client.Object);

            Assert.ThrowsAsync<ArgumentNullException>(async () => { 
                await sendGridService.SendAsync(null, new MailAddress("test2@address.com"), "subject", "content", CancellationToken.None); 
            });
        }

        [Test]
        public void ThrowsOnNullRecipientEmailAddress()
        {
            _client.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None))
                .Returns(Task.FromResult<Response>(new Response(HttpStatusCode.OK, null, null)));

            var sendGridService = new SendGridEmailService(_logger.Object, _client.Object);

            Assert.ThrowsAsync<ArgumentNullException>(async () => { 
                await sendGridService.SendAsync(new MailAddress("test@address.com"), null, "subject", "content", CancellationToken.None); 
            });
        }

    }
}
