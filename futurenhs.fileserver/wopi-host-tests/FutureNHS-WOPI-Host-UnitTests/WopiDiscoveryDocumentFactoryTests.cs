using FutureNHS.WOPIHost;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS_WOPI_Host_UnitTests.Stubs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests
{
    [TestClass]
    public sealed class WopiDiscoveryDocumentFactoryTests
    {
        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task CreateDocumentAsync_ThrowsWhenCancellationTokenCancelled()
        {
            using var cts = new CancellationTokenSource();

            cts.Cancel();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentFactory>>().Object; 
            
            var memoryCache = new Moq.Mock<IMemoryCache>().Object;

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>().Object;

            IWopiDiscoveryDocumentFactory wopiDiscoveryDocumentFactory = new WopiDiscoveryDocumentFactory(memoryCache, wopiDiscoveryDocumentRepository, logger);

            _ = await wopiDiscoveryDocumentFactory.CreateDocumentAsync(cts.Token);
        }




        [TestMethod]
        public async Task CreateDocumentAsync_PullsDocumentFromCacheWhenOneIsAvailable()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentFactory>>().Object;

            var services = new ServiceCollection();

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

            var cachedWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>();

            cachedWopiDiscoveryDocument.SetupGet(x => x.IsTainted).Returns(false);

            memoryCache.Set(ExtensionMethods.WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, cachedWopiDiscoveryDocument.Object);

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>().Object;

            IWopiDiscoveryDocumentFactory wopiDiscoveryDocumentFactory = new WopiDiscoveryDocumentFactory(memoryCache, wopiDiscoveryDocumentRepository, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentFactory.CreateDocumentAsync(cancellationToken);

            Assert.AreSame(cachedWopiDiscoveryDocument.Object, wopiDiscoveryDocument, "Expected the cached document to have been returned");
        }

        [TestMethod]
        public async Task CreateDocumentAsync_DoesNotPullDocumentFromCacheWhenOneIsAvailableButIsMarkedAsTainted()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentFactory>>().Object;

            var taintedWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>();

            taintedWopiDiscoveryDocument.SetupGet(x => x.IsTainted).Returns(true);

            var services = new ServiceCollection();

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

            memoryCache.Set(ExtensionMethods.WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, taintedWopiDiscoveryDocument.Object);

            var untaintedWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>().Object;

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>();

            wopiDiscoveryDocumentRepository.Setup(x => x.GetAsync(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(untaintedWopiDiscoveryDocument));

            IWopiDiscoveryDocumentFactory wopiDiscoveryDocumentFactory = new WopiDiscoveryDocumentFactory(memoryCache, wopiDiscoveryDocumentRepository.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentFactory.CreateDocumentAsync(cancellationToken);

            Assert.AreNotSame(taintedWopiDiscoveryDocument, wopiDiscoveryDocument, "Expected the tainted document to have been discarded and a new one generated");

            Assert.AreSame(untaintedWopiDiscoveryDocument, wopiDiscoveryDocument, "Expected the untainted document to have been returned by the repository");
        }

        [TestMethod]
        public async Task CreateDocumentAsync_DefersToDiscoveryDocumentRepositoryToGetNewDocument()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentFactory>>().Object;

            var services = new ServiceCollection();

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

            var newWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>().Object;

            var discoveryDocumentRepositoryInvoked = false;

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>();

            wopiDiscoveryDocumentRepository.Setup(x => x.GetAsync(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(newWopiDiscoveryDocument)).Callback(() => { discoveryDocumentRepositoryInvoked = true; });

            IWopiDiscoveryDocumentFactory wopiDiscoveryDocumentFactory = new WopiDiscoveryDocumentFactory(memoryCache, wopiDiscoveryDocumentRepository.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentFactory.CreateDocumentAsync(cancellationToken);

            Assert.IsTrue(discoveryDocumentRepositoryInvoked, "Expected the wopi discovery document repository to have been asked to provide a new document");

            Assert.AreSame(newWopiDiscoveryDocument, wopiDiscoveryDocument, "Expected the tainted document to have been discarded and a new one generated");
        }

        [TestMethod]
        public async Task CreateDocumentAsync_ReturnsPrimedDocumentWhenDiscoveryEndpointReturnsValidResponse()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<WopiDiscoveryDocumentFactory>>().Object;

            var services = new ServiceCollection();

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

            var sourceEndpoint = new Uri(WopiDiscoveryDocumentTests.WOPI_ROOT + WopiDiscoveryDocumentTests.WOPI_DISCOVERY_DOCUMENT_URL, UriKind.Absolute);

            var wopiConfiguration = new WopiConfiguration() { ClientDiscoveryDocumentUrl = sourceEndpoint };

            var wopiConfigurationOptionsSnapshot = new Moq.Mock<IOptionsSnapshot<WopiConfiguration>>();

            wopiConfigurationOptionsSnapshot.SetupGet(x => x.Value).Returns(wopiConfiguration);

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(WopiDiscoveryDocumentTests.WOPI_DISCOVERY_DOCUMENT_XML, Encoding.UTF8, "application/xml")
            };

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandler, true);

            httpClientFactory.Setup(x => x.CreateClient("wopi-discovery-document")).Returns(httpClient);

            var newWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>().Object;

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>();

            wopiDiscoveryDocumentRepository.Setup(x => x.GetAsync(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(newWopiDiscoveryDocument));

            IWopiDiscoveryDocumentFactory wopiDiscoveryDocumentFactory = new WopiDiscoveryDocumentFactory(memoryCache, wopiDiscoveryDocumentRepository.Object, logger);

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentFactory.CreateDocumentAsync(cancellationToken);

            Assert.IsFalse(wopiDiscoveryDocument.IsEmpty, "Expected a none empty document to be returned when the discovery endpoint is known and returns a valid response");
        }
    }
}
