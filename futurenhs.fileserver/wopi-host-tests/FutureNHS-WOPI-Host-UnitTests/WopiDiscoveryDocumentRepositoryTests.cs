using FutureNHS.WOPIHost;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS_WOPI_Host_UnitTests.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests
{
    [TestClass]
    public sealed class WopiDiscoveryDocumentRepositoryTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CTor_ThrowsWhenHttpClientFactoryIsNull()
        {
            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>().Object;

            _ = new WopiDiscoveryDocumentRepository(default, wopiConfigurationOptionsSnapshot, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CTor_ThrowsWhenConfigSnapshotIsNull()
        {
            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>().Object;

            _ = new WopiDiscoveryDocumentRepository(httpClientFactory, default, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CTor_ThrowsWhenConfigurationIsMissing()
        {
            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>().Object;

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.Setup(x => x.Value).Returns(default(WopiConfiguration));

            _ = new WopiDiscoveryDocumentRepository(httpClientFactory, wopiConfigurationOptionsSnapshot.Object, logger);
        }

        [TestMethod]
        public void CTor_DoesNotThrowWhenLoggerIsNull()
        {
            var httpClientFactory = new Moq.Mock<IHttpClientFactory>().Object;

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = new Uri("https://absolute.uri") };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.Setup(x => x.Value).Returns(wopiConfiguration);

            _ = new WopiDiscoveryDocumentRepository(httpClientFactory, wopiConfigurationOptionsSnapshot.Object, default);
        }




        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task GetAsync_ThrowsWhenCancellationTokenCancelled()
        {
            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = new Uri("https://absolute.uri") };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.Setup(x => x.Value).Returns(wopiConfiguration);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>().Object;

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory, wopiConfigurationOptionsSnapshot.Object, logger);

            using var cts = new CancellationTokenSource();

            cts.Cancel();

            _ = await wopiDiscoveryDocumentRepository.GetAsync(cts.Token);
        }

        [TestMethod]
        public async Task GetAsync_ConstructsExpectedRequestToWopiClient()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + "client/hosting/discovery", UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("<xml></xml>", Encoding.UTF8, "application/xml")
            };

            var messageHandlerInvoked = false;

            var httpMessageHandler = new HttpMessageHandlerStub(
                (request, _) => {

                    messageHandlerInvoked = true;

                    Assert.AreEqual(sourceEndpoint, request.RequestUri, "Expected the supplied endpoint to be used to retrieve the document");

                    Assert.IsTrue(request.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/xml")), "Expected the accept header to be set to retrieve an xml document");
                    Assert.IsTrue(request.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue("text/xml")), "Expected the accept header to be set to retrieve an xml document");

                    Assert.AreEqual(HttpMethods.Get, request.Method.Method, "Expected the document to generate a GET request to the discovery document endpoint");

                    return httpResponseMessage;
                });

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: true);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory.Object, wopiConfigurationOptionsSnapshot.Object, logger);

            await wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

            Assert.IsTrue(messageHandlerInvoked, "Expected the stubbed request message handler to have been invoked to retrieve the xml document from the WOPI client");
        }

        [TestMethod]
        public async Task GetAsync_ReturnsEmptyDocumentIfFailureStatusCodeReturnedFromWopiClient()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + "client/hosting/discovery", UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: true);

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory.Object, wopiConfigurationOptionsSnapshot.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

            Assert.AreSame(WopiDiscoveryDocument.Empty, wopiDiscoveryDocument);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsEmptyDocumentIfResponseContentTypeHeaderIsMissing()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + "client/hosting/discovery", UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("<xml></xml>", Encoding.UTF8, "application/xml")
            };

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: true);

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory.Object, wopiConfigurationOptionsSnapshot.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

            Assert.AreSame(WopiDiscoveryDocument.Empty, wopiDiscoveryDocument);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsEmptyDocumentIfResponseContentTypeHeaderIsNotSupported()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + "client/hosting/discovery", UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("<xml></xml>", Encoding.UTF8, "application/xml")
            };

            httpResponseMessage.Headers.Add("ContentType", "unsupported/contenttype");

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: true);

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory.Object, wopiConfigurationOptionsSnapshot.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

            Assert.AreSame(WopiDiscoveryDocument.Empty, wopiDiscoveryDocument);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsEmptyDocumentForInvalidWopiClientXmlResponse()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + "client/hosting/discovery", UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("<xml></xml>", Encoding.UTF8, "application/xml")
            };

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: true);

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory.Object, wopiConfigurationOptionsSnapshot.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

            Assert.IsTrue(wopiDiscoveryDocument.IsEmpty);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsEmptyDocumentIfConnectionToWopiClientCannotBeEstablished()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + "client/hosting/discovery", UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => throw new HttpRequestException("Mimicing failure to connect to remote host"));

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: true);

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory.Object, wopiConfigurationOptionsSnapshot.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

            Assert.IsTrue(wopiDiscoveryDocument.IsEmpty);
        }

        [TestMethod]
        public async Task GetAsync_ReturnsPrimedDocumentForValidWopiClientXmlResponse()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentRepository>>().Object;

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + WopiDiscoveryDocumentTests.WOPI_DISCOVERY_DOCUMENT_URL, UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(WopiDiscoveryDocumentTests.WOPI_DISCOVERY_DOCUMENT_XML, Encoding.UTF8, "application/xml")
            };

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: true);

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            IWopiDiscoveryDocumentRepository wopiDiscoveryDocumentRepository = new WopiDiscoveryDocumentRepository(httpClientFactory.Object, wopiConfigurationOptionsSnapshot.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentRepository.GetAsync(cancellationToken);

            Assert.IsNotNull(wopiDiscoveryDocument);
            Assert.IsFalse(wopiDiscoveryDocument.IsEmpty);
            Assert.IsFalse(wopiDiscoveryDocument.IsTainted);
        }
    }
}
