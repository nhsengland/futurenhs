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
    public sealed class WopiRequestFactoryTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ThrowsIfUserAuthenticationServiceIsNull()
        {
            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(new Features());

            _ = new WopiRequestHandlerFactory(userAuthenticationService: default, userFileMetadataProvider.Object, features: snapshot.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ThrowsIfUserFileMetadataProviderIsNull()
        {
            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(new Features());

            _ = new WopiRequestHandlerFactory(userAuthenticationService: userAuthenticationService.Object, userFileMetadataProvider: default, features: snapshot.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ThrowsIfFeaturesOptionsConfigurationIsNull()
        {
            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            _ = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ThrowsIfFeaturesConfigurationIsNull()
        {
            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(default(Features));

            _ = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: snapshot.Object);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task CreateRequestHandlerAsync_ThrowsIfHttpContextIsNull()
        {
            var cancellationToken = CancellationToken.None;

            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

            var features = new Features();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(features);

            IWopiRequestHandlerFactory wopiRequestFactory = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: snapshot.Object);

            _ = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext: default, cancellationToken);
        }

        [TestMethod]
        public async Task CreateRequestHandlerAsync_NoneWopiRequestIsIdentifiedAndIgnored()
        {
            var cancellationToken = CancellationToken.None;

            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

            var features = new Features();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(features);

            IWopiRequestHandlerFactory wopiRequestFactory = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: snapshot.Object);

            var httpContext = new DefaultHttpContext();

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            Assert.IsNotNull(wopiRequestHandler);
            
            Assert.IsTrue(wopiRequestHandler.IsEmpty, "Expected a none WOPI request to be ignored and return an empty marker");
        }

        [TestMethod]
        public async Task CreateRequestHandlerAsync_WopiRequestWithMissingAccessTokenIsIdentifiedAndIgnored()
        {
            var cancellationToken = CancellationToken.None;

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

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(httpContext, file, cancellationToken)).Returns(Task.FromResult(default(AuthenticatedUser)));

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            Assert.IsNotNull(wopiRequestHandler);

            Assert.IsTrue(wopiRequestHandler.IsEmpty, "Expected a WOPI request with a missing access token to be ignored and return an empty marker");
        }

        [TestMethod]
        public async Task CreateRequesHandlert_WopiRequestWithInvalidAccessTokenIsIdentifiedAndIgnored()
        {
            var cancellationToken = CancellationToken.None;

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
            httpRequest.QueryString = new QueryString("?access_token=");

            httpRequest.Headers["X-WOPI-ItemVersion"] = file.Version;

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(httpContext, file, cancellationToken)).Returns(Task.FromResult(default(AuthenticatedUser)));

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            Assert.IsNotNull(wopiRequestHandler);

            Assert.IsTrue(wopiRequestHandler.IsEmpty, "Expected a WOPI request with an invalid access token to be ignored and return an empty marker");
        }

        [TestMethod]
        public async Task CreateRequestHandlerAsync_IdentifiesCheckFileInfoRequest()
        {
            var cancellationToken = CancellationToken.None;

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

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(httpContext, file, cancellationToken)).Returns(Task.FromResult(authenticatedUser));

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            Assert.IsNotNull(wopiRequestHandler);

            Assert.IsInstanceOfType(wopiRequestHandler, typeof(CheckFileInfoWopiRequestHandler), "Expected Check File Info requests to be identified");
        }

        [TestMethod]
        public async Task CreateRequestHandlerAsync_IdentifiesGetFileRequest()
        {
            var cancellationToken = CancellationToken.None;

            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

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
                 ContentHash = Convert.FromBase64String("aGFzaA == "),
                 UserHasViewPermission = true,
                 UserHasEditPermission = false
                 };

            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default) { FileMetadata = fileMetadata };

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<File>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(authenticatedUser));

            userFileMetadataProvider.Setup(x => x.GetForFileAsync(fileMetadata.AsFile(), authenticatedUser, cancellationToken)).Returns(Task.FromResult(fileMetadata));

            var features = new Features();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(features);

            IWopiRequestHandlerFactory wopiRequestFactory = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: snapshot.Object);

            var file = fileMetadata.AsFile();

            var httpContext = new DefaultHttpContext();

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Get;
            httpRequest.Path = $"/wopi/files/{file.Id}/contents";
            httpRequest.QueryString = new QueryString("?access_token=9747f3e6-c18d-47b5-bd86-56899cbf9d4a");

            httpRequest.Headers["X-WOPI-ItemVersion"] = file.Version;

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(httpContext, file, cancellationToken)).Returns(Task.FromResult(authenticatedUser));

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            Assert.IsNotNull(wopiRequestHandler);

            Assert.IsInstanceOfType(wopiRequestHandler, typeof(GetFileWopiRequestHandler), "Expected Get File requests to be identified");
        }

        [TestMethod]
        public async Task CreateRequestHandlerAsync_IdentifiesSaveFileRequest()
        {
            var cancellationToken = CancellationToken.None;

            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

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
                 ContentHash = Convert.FromBase64String("aGFzaA == "),
                 UserHasViewPermission = true,
                 UserHasEditPermission = false
                 };

            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default) { FileMetadata = fileMetadata };

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<File>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(authenticatedUser));

            userFileMetadataProvider.Setup(x => x.GetForFileAsync(fileMetadata.AsFile(), authenticatedUser, cancellationToken)).Returns(Task.FromResult(fileMetadata));

            var features = new Features();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(features);

            IWopiRequestHandlerFactory wopiRequestFactory = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: snapshot.Object);

            var file = fileMetadata.AsFile();

            var httpContext = new DefaultHttpContext();

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Post;
            httpRequest.Path = $"/wopi/files/{file.Id}/contents";
            httpRequest.QueryString = new QueryString("?access_token=9747f3e6-c18d-47b5-bd86-56899cbf9d4a");

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(httpContext, file, cancellationToken)).Returns(Task.FromResult(authenticatedUser));

            var wopiRequestHandler = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            Assert.IsNotNull(wopiRequestHandler);

            Assert.IsInstanceOfType(wopiRequestHandler, typeof(PostFileWopiRequestHandler), "Expected Save File requests to be identified");
        }

        [TestMethod]
        public async Task CreateRequestHandlerAsync_IdentifiesAuthUserRequest()
        {
            var cancellationToken = CancellationToken.None;

            var userAuthenticationService = new Moq.Mock<IUserAuthenticationService>();

            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default);

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(Moq.It.IsAny<HttpContext>(), Moq.It.IsAny<File>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(authenticatedUser));

            var userFileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

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
                ContentHash = Convert.FromBase64String("aGFzaA == "),
                UserHasViewPermission = true,
                UserHasEditPermission = false
                };

            userFileMetadataProvider.Setup(x => x.GetForFileAsync(fileMetadata.AsFile(), authenticatedUser, cancellationToken)).Returns(Task.FromResult(fileMetadata));

            var features = new Features();

            var snapshot = new Moq.Mock<IOptionsSnapshot<Features>>();

            snapshot.SetupGet(x => x.Value).Returns(features);

            IWopiRequestHandlerFactory wopiRequestFactory = new WopiRequestHandlerFactory(userAuthenticationService.Object, userFileMetadataProvider.Object, features: snapshot.Object);

            var file = fileMetadata.AsFile();

            var httpContext = new DefaultHttpContext();

            var httpRequest = httpContext.Request;

            httpRequest.Method = HttpMethods.Post;
            httpRequest.Path = $"/wopi/files/{file.Id}/authorise-user";
            httpRequest.QueryString = new QueryString("?permission=view");

            userAuthenticationService.Setup(x => x.GetForFileContextAsync(httpContext, file, cancellationToken)).Returns(Task.FromResult(authenticatedUser));

            var wopiRequest = await wopiRequestFactory.CreateRequestHandlerAsync(httpContext, cancellationToken);

            Assert.IsNotNull(wopiRequest);

            Assert.IsInstanceOfType(wopiRequest, typeof(AuthoriseUserRequestHandler), "Expected Authorise User requests to be identified");
        }
    }
}
