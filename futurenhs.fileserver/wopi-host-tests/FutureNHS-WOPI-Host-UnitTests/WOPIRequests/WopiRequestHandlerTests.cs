using FutureNHS.WOPIHost;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS.WOPIHost.WOPIRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests
{
    [TestClass]
    public sealed class WopiRequestHandlerTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task HandleAsync_ThrowsIfWopiRequestIsEmpty()
        {
            var cancellationToken = new CancellationToken();

            var httpContext = new DefaultHttpContext();

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Get;
            httpRequest.Path = "/wopi/files/file-name|file-version";
            httpRequest.QueryString = new QueryString("?access_token=<expired-access-token>");

            var wopiRequest = WopiRequestHandler.Empty;

            await wopiRequest.HandleAsync(httpContext, cancellationToken);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task HandleAsync_ThrowsWhenCancellationTokenCancelled()
        {
            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();
            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

            var features = new Features();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(features);

            IWopiRequestHandlerFactory wopiRequestFactory = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: snapshot.Object);

            var file = File.FromId("file-name|file-version");

            var httpContext = new DefaultHttpContext();

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Get;
            httpRequest.Path = $"/wopi/files/{file.Id}";
            httpRequest.QueryString = new QueryString("?access_token=9747f3e6-c18d-47b5-bd86-56899cbf9d4a");

            httpRequest.Headers["X-WOPI-ItemVersion"] = file.Version;

            var authenticatedUser = new AuthenticatedUser(Guid.Parse("9747f3e6-c18d-47b5-bd86-56899cbf9d4a"), default);

            using var cts = new CancellationTokenSource();

            var cancellationToken = cts.Token;

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(httpContext, file, cancellationToken)).Returns(Task.FromResult(authenticatedUser));

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            cts.Cancel();

            await wopiRequestHandler.HandleAsync(httpContext, cancellationToken);
        }
    }
}
