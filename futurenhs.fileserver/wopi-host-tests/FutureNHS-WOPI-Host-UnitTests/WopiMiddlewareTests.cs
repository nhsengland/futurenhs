using FutureNHS.WOPIHost;
using FutureNHS.WOPIHost.Azure;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS.WOPIHost.WOPIRequests;
using FutureNHS_WOPI_Host_UnitTests.Stubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests
{
    [TestClass]
    public sealed class WopiMiddlewareTests
    {
        [TestMethod]
        public void CTor_DoesNotThrowIfNextItemInPipelineIsNull()
        {
            _ = new WopiMiddleware(next: default);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Invoke_ThrowsIfHttpContextIsNull()
        {
            var wopiMiddleware = new WopiMiddleware(next: default);

            await wopiMiddleware.Invoke(httpContext: default);
        }

        [TestMethod]
        public async Task Invoke_CallsNextMiddlewareInPipelineIfInjected()
        {
            var configurationData = new Dictionary<string, string>() {
                { "App:MvcForumUserInfoUrl", "https://my.absolute.url/" }
            };

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(configurationData);

            var configuration = configurationBuilder.Build();
            
            var services = new ServiceCollection();

            services.Configure<AppConfiguration>(configuration.GetSection("App"));

            services.AddScoped<ISystemClock, SystemClock>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<IWopiRequestHandlerFactory, WopiRequestHandlerFactory>();
            services.AddScoped(sp => new Moq.Mock<IAzureTableStoreClient>().Object);
            services.AddScoped(sp => new Moq.Mock<IUserFileAccessTokenRepository>().Object);
            services.AddScoped(sp => new Moq.Mock<IUserFileMetadataProvider>().Object);
            services.AddScoped(sp => new Moq.Mock<IHttpClientFactory>().Object);
            services.AddScoped(sp => new Moq.Mock<ILogger<UserFileAccessTokenRepository>>().Object);
            services.AddScoped(sp => new Moq.Mock<ILogger<UserAuthenticationService>>().Object);
            services.AddScoped(sp => new Moq.Mock<ILogger<AzureTableStoreClient>>().Object);

            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

            var hasBeenInvoked = false;

            var wopiMiddleware = new WopiMiddleware(_ => { hasBeenInvoked = true; return Task.CompletedTask; });

            await wopiMiddleware.Invoke(httpContext);

            Assert.IsTrue(hasBeenInvoked, "Expected Wopi Middleware to execute the next item in the pipeline after it completes its work");
        }



        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task Invoke_ProcessRequest_ThrowsWhenCancellationTokenCancelled()
        {
            using var cts = new CancellationTokenSource();

            cts.Cancel();

            var httpContext = new DefaultHttpContext() { RequestAborted = cts.Token };

            var wopiMiddleware = new WopiMiddleware(default);

            await wopiMiddleware.Invoke(httpContext);
        }

        [TestMethod]
        public async Task Invoke_ProcessRequest_DefersToWopiRequestFactoryToIdentifyWopiRequests()
        {
            var configurationData = new Dictionary<string, string>();

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(configurationData);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            var requestFactoryInvoked = false;

            var wopiRequestFactory = new Moq.Mock<IWopiRequestHandlerFactory>();

            var emptyWopiRequest = WopiRequestHandler.Empty;

            wopiRequestFactory.Setup(x => x.CreateRequestHandlerAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<CancellationToken>()))
                              .Returns(Task.FromResult(WopiRequestHandler.Empty))
                              .Callback(() => { requestFactoryInvoked = true; });

            services.AddScoped(sp => wopiRequestFactory.Object);

            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

            var wopiMiddleware = new WopiMiddleware(default);

            await wopiMiddleware.Invoke(httpContext);

            Assert.IsTrue(requestFactoryInvoked, "Expected the wopi request factory to have been deferred to such that it could identify if the request was WOPI related or not");
        }


        [TestMethod]
        public async Task Invoke_ProcessRequest_DefersToWopiProofCheckerToVerifyPresentedProofs()
        {
            var configurationData = new Dictionary<string, string>();

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(configurationData);

            var configuration = configurationBuilder.Build();

            WopiRequestHandler wopiRequestHandlerStub = new WopiRequestHandlerStub((_, __) => Task.FromResult(StatusCodes.Status200OK));

            var wopiRequestFactory = new Moq.Mock<IWopiRequestHandlerFactory>();

            wopiRequestFactory.Setup(x => x.CreateRequestHandlerAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<CancellationToken>()))
                              .Returns(Task.FromResult(wopiRequestHandlerStub));

            var proofCheckerInvoked = false;

            var wopiCryptoProofChecker = new Moq.Mock<IWopiCryptoProofChecker>();

            wopiCryptoProofChecker.Setup(x => x.IsProofInvalid(Moq.It.IsAny<HttpRequest>(), Moq.It.IsAny<IWopiProofKeysProvider>())).Returns((false, false)).Callback(() => { proofCheckerInvoked = true; });

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>();

            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddScoped(sp => wopiDiscoveryDocumentRepository.Object);
            services.AddScoped(sp => wopiRequestFactory.Object);
            services.AddScoped(sp => wopiCryptoProofChecker.Object);

            services.AddScoped<IWopiDiscoveryDocumentFactory, WopiDiscoveryDocumentFactory>(); 

            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

            var cachedWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>();

            cachedWopiDiscoveryDocument.SetupGet(x => x.IsTainted).Returns(false);

            memoryCache.Set(ExtensionMethods.WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, cachedWopiDiscoveryDocument.Object);

            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Get;
            httpRequest.Path = "/wopi/files/file-name|file-version";
            httpRequest.QueryString = new QueryString("?access_token=tokengoeshere");

            var wopiMiddleware = new WopiMiddleware(default);

            await wopiMiddleware.Invoke(httpContext);

            Assert.IsTrue(proofCheckerInvoked, "Expected the wopi proof checker to have been deferred to such that it could validate the proof presented in the request");
        }

        [TestMethod]
        public async Task Invoke_ProcessRequest_TransparentlyIgnoresNoneWopiRequests()
        {
            var configurationData = new Dictionary<string, string>() {
                { "App:MvcForumUserInfoUrl", "https://my.absolute.url/" }
            };

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(configurationData);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.Configure<AppConfiguration>(configuration.GetSection("App"));

            services.AddScoped<ISystemClock, SystemClock>();
            services.AddScoped<IWopiRequestHandlerFactory, WopiRequestHandlerFactory>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();

            services.AddScoped(sp => new Moq.Mock<IAzureTableStoreClient>().Object);
            services.AddScoped(sp => new Moq.Mock<IUserFileAccessTokenRepository>().Object);
            services.AddScoped(sp => new Moq.Mock<IUserFileMetadataProvider>().Object);
            services.AddScoped(sp => new Moq.Mock<IHttpClientFactory>().Object);
            services.AddScoped(sp => new Moq.Mock<ILogger<UserAuthenticationService>>().Object);

            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

            var wopiMiddleware = new WopiMiddleware(default);

            await wopiMiddleware.Invoke(httpContext);
        }

        [TestMethod]
        public async Task Invoke_ProcessRequest_InvokesRequestHandlerIfTheProofIsVerified()
        {
            var configurationData = new Dictionary<string, string>();

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(configurationData);

            var configuration = configurationBuilder.Build();

            var wopiRequestHandlerInvoked = false;

            WopiRequestHandler wopiRequestHandlerStub = new WopiRequestHandlerStub((_, __) => { wopiRequestHandlerInvoked = true; return Task.FromResult(StatusCodes.Status200OK); });

            var wopiRequestFactory = new Moq.Mock<IWopiRequestHandlerFactory>();

            wopiRequestFactory.Setup(x => x.CreateRequestHandlerAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<CancellationToken>()))
                              .Returns(Task.FromResult(wopiRequestHandlerStub));

            var wopiCryptoProofChecker = new Moq.Mock<IWopiCryptoProofChecker>();

            wopiCryptoProofChecker.Setup(x => x.IsProofInvalid(Moq.It.IsAny<HttpRequest>(), Moq.It.IsAny<IWopiProofKeysProvider>())).Returns((false, false));

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>();

            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddScoped(sp => wopiDiscoveryDocumentRepository.Object);
            services.AddScoped(sp => wopiRequestFactory.Object);
            services.AddScoped(sp => wopiCryptoProofChecker.Object);

            services.AddScoped<IWopiDiscoveryDocumentFactory, WopiDiscoveryDocumentFactory>();

            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

            var cachedWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>();

            cachedWopiDiscoveryDocument.SetupGet(x => x.IsTainted).Returns(false);

            memoryCache.Set(ExtensionMethods.WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, cachedWopiDiscoveryDocument.Object);

            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Get;
            httpRequest.Path = "/wopi/files/file-name|file-version";
            httpRequest.QueryString = new QueryString("?access_token=tokengoeshere");

            var wopiMiddleware = new WopiMiddleware(default);

            await wopiMiddleware.Invoke(httpContext);

            Assert.IsTrue(wopiRequestHandlerInvoked, "Expected the request handler supplied by the factory to be invoked");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public async Task Invoke_ProcessRequest_ThrowsIfOfferedProofIsNotVerifiedToBeAuthentic()
        {
            var configurationData = new Dictionary<string, string>() {
                { "App:MvcForumUserInfoUrl", "https://my.absolute.url/" }
            };

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(configurationData);

            var configuration = configurationBuilder.Build();

            var wopiCryptoProofChecker = new Moq.Mock<IWopiCryptoProofChecker>();

            wopiCryptoProofChecker.Setup(x => x.IsProofInvalid(Moq.It.IsAny<HttpRequest>(), Moq.It.IsAny<IWopiProofKeysProvider>())).Returns((true, false));

            var wopiDiscoveryDocumentRepository = new Moq.Mock<IWopiDiscoveryDocumentRepository>();

            var fileRepository = new Moq.Mock<IUserFileMetadataProvider>();

            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddHttpClient();

            services.Configure<AppConfiguration>(configuration.GetSection("App"));

            services.AddScoped<ISystemClock, SystemClock>();
            services.AddScoped<IWopiRequestHandlerFactory, WopiRequestHandlerFactory>();
            services.AddScoped<IWopiDiscoveryDocumentFactory, WopiDiscoveryDocumentFactory>();

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"Id\":\"9747f3e6-c18d-47b5-bd86-56899cbf9d4a\" }", Encoding.UTF8, "application/json")
            };

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: false);

            httpClientFactory.Setup(x => x.CreateClient("mvcforum-userinfo")).Returns(httpClient);

            services.AddScoped(sp => httpClientFactory.Object);
            services.AddScoped(sp => new Moq.Mock<ILogger<UserAuthenticationService>>().Object);

            services.AddScoped(sp => wopiCryptoProofChecker.Object);
            services.AddScoped(sp => wopiDiscoveryDocumentRepository.Object);
            services.AddScoped(sp => fileRepository.Object);

            var fileMetadata = new UserFileMetadata() {
                FileId = Guid.NewGuid(), 
                Title = "title", 
                Description = "description",
                GroupName = "groupName",
                FileVersion = "version",
                OwnerUserName = "owner",
                Name = "name",
                Extension = ".ext",
                SizeInBytes = 1,
                BlobName = Guid.NewGuid().ToString() + ".ext",
                LastWriteTimeUtc = DateTimeOffset.UtcNow.AddDays(-1),
                ContentHash = Convert.FromBase64String("aGFzaA=="),
                UserHasViewPermission = true, 
                UserHasEditPermission = false
                };

            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default) { FileMetadata = fileMetadata };

            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<File>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(authenticatedUser));

            services.AddScoped<IUserAuthenticationService>(sp => userAuthenticationService.Object);

            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

            var cachedWopiDiscoveryDocument = new Moq.Mock<IWopiDiscoveryDocument>();

            cachedWopiDiscoveryDocument.SetupGet(x => x.IsTainted).Returns(false);

            memoryCache.Set(ExtensionMethods.WOPI_DISCOVERY_DOCUMENT_CACHE_KEY, cachedWopiDiscoveryDocument.Object);

            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Get;
            httpRequest.Path = $"/wopi/files/{fileMetadata.AsFile().Id}/contents";
            httpRequest.QueryString = new QueryString("?access_token=9747f3e6-c18d-47b5-bd86-56899cbf9d4a");

            var wopiMiddleware = new WopiMiddleware(default);

            await wopiMiddleware.Invoke(httpContext);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public async Task Invoke_ProcessRequest_ThrowsIfDiscoveryDocumentCannotBeLocatedToExtractProofKeys()
        {
            var configurationData = new Dictionary<string, string>() {
                { "App:MvcForumUserInfoUrl", "https://my.absolute.url/" }
            };

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(configurationData);

            var configuration = configurationBuilder.Build();

            var wopiDiscoveryDocumentFactory = new Moq.Mock<IWopiDiscoveryDocumentFactory>();

            wopiDiscoveryDocumentFactory.Setup(x => x.CreateDocumentAsync(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult<IWopiDiscoveryDocument>(WopiDiscoveryDocument.Empty));

            var fileRepository = new Moq.Mock<IUserFileMetadataProvider>();

            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddHttpClient();

            services.Configure<AppConfiguration>(configuration.GetSection("App"));

            services.AddScoped<ISystemClock, SystemClock>();
            services.AddScoped<IWopiRequestHandlerFactory, WopiRequestHandlerFactory>();
            services.AddScoped<IWopiDiscoveryDocumentFactory, WopiDiscoveryDocumentFactory>();

            var httpClientFactory = new Moq.Mock<IHttpClientFactory>();

            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"Id\":\"9747f3e6-c18d-47b5-bd86-56899cbf9d4a\" }", Encoding.UTF8, "application/json")
            };

            var httpMessageHandler = new HttpMessageHandlerStub((request, _) => httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandler, disposeHandler: false);

            httpClientFactory.Setup(x => x.CreateClient("mvcforum-userinfo")).Returns(httpClient);

            services.AddScoped(sp => httpClientFactory.Object);
            services.AddScoped(sp => new Moq.Mock<ILogger<UserAuthenticationService>>().Object);

            services.AddScoped(sp => fileRepository.Object);

            services.AddScoped<IWopiRequestHandlerFactory, WopiRequestHandlerFactory>();

            services.AddScoped(sp => wopiDiscoveryDocumentFactory.Object);

            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default);

            var userAuthentcationService = new Moq.Mock<IUserAuthenticationService>();

            userAuthentcationService.Setup(x => x.GetForFileContextAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<File>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(authenticatedUser));

            services.AddScoped<IUserAuthenticationService>(sp => userAuthentcationService.Object);

            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Get;
            httpRequest.Path = "/wopi/files/file-name|file-version";
            httpRequest.QueryString = new QueryString("?access_token=9747f3e6-c18d-47b5-bd86-56899cbf9d4a");

            var wopiMiddleware = new WopiMiddleware(default);

            await wopiMiddleware.Invoke(httpContext);
        }
    }
}
