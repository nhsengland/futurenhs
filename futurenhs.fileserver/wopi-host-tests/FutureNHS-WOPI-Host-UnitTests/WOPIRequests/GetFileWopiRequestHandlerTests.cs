using FutureNHS.WOPIHost;
using FutureNHS.WOPIHost.WOPIRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using File = FutureNHS.WOPIHost.File;

namespace FutureNHS_WOPI_Host_UnitTests.WOPIRequests
{
    [TestClass]
    public sealed class GetFileWopiRequestHandlerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void With_ThrowsIfAuthenticatedUserIsNull()
        {
            var file = File.FromId(Guid.NewGuid().ToString());

            GetFileWopiRequestHandler.With(default, file);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void With_ThrowsIfFileIsEmpty()
        {
            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default);

            GetFileWopiRequestHandler.With(authenticatedUser, File.Empty);
        }
        [TestMethod]
        [DataRow("Excel-Spreadsheet.xlsx")]
        [DataRow("Image-File.jpg")]
        [DataRow("OpenDocument-Text-File.odt")]
        [DataRow("Portable-Document-Format-File.pdf")]
        [DataRow("PowerPoint-Presentation.pptx")]
        [DataRow("Text-File.txt")]
        [DataRow("Word-Document.docx")]
        public async Task HandleAsync_ResolvesAndWritesFileCorrectlyToGivenStream(string fileName)
        {
            var cancellationToken = new CancellationToken();

            var httpContext = new DefaultHttpContext();

            var contentRootPath = Environment.CurrentDirectory;

            var filePath = Path.Combine(contentRootPath, "Files", fileName);

            Assert.IsTrue(System.IO.File.Exists(filePath), $"Expected the {fileName} file to be accessible in the test environment");

            var fileInfo = new FileInfo(filePath);

            var fileBuffer = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);

            using var responseBodyStream = new MemoryStream(fileBuffer.Length);
            
            httpContext.Response.Body = responseBodyStream;

            var fileContentMetadataRepository = new Moq.Mock<IFileContentMetadataRepository>();

            var fileRepositoryInvoked = false;

            var services = new ServiceCollection();

            services.AddScoped(sp => fileContentMetadataRepository.Object);

            var fileVersion = Guid.NewGuid().ToString();

            using var algo = MD5.Create();

            var contentHash = algo.ComputeHash(fileBuffer);

            var fileId = Guid.NewGuid();

            var fileMetadata = new UserFileMetadata() {
                FileId = fileId,
                FileVersion = fileVersion,
                Title = "title",
                Description = "description",
                Name = fileName,
                SizeInBytes = (ulong)fileInfo.Length,
                Extension = fileInfo.Extension,
                ContentHash = Convert.FromBase64String("aGFzaA == "),
                GroupName = "groupName",
                OwnerUserName = "owner",
                BlobName = "blobName",
                LastWriteTimeUtc = DateTimeOffset.UtcNow,
                UserHasViewPermission = true,
                UserHasEditPermission = false
                };

            var fileWriteDetails = new FileContentMetadata(fileVersion, "content-type", contentHash, (ulong)fileBuffer.Length, "content-encoding", "content-language", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, fileMetadata);

            fileContentMetadataRepository.
                Setup(x => x.GetDetailsAndPutContentIntoStreamAsync(Moq.It.IsAny<UserFileMetadata>(), Moq.It.IsAny<Stream>(), Moq.It.IsAny<CancellationToken>())).
                Callback(async (UserFileMetadata givenFileMetadata, Stream givenStream, CancellationToken givenCancellationToken) => {

                    Assert.IsNotNull(givenFileMetadata);
                    Assert.IsNotNull(givenStream);

                    Assert.IsFalse(givenCancellationToken.IsCancellationRequested, "Expected the cancellation token to not be cancelled");

                    Assert.AreEqual(responseBodyStream, givenStream, "Expected the SUT to as the repository to write the file to the stream it was asked to");
                    Assert.AreEqual(fileId, givenFileMetadata.FileId, "Expected the SUT to request the file from the repository whose id it was provided with");
                    Assert.AreEqual(fileName, givenFileMetadata.Name, "Expected the SUT to request the file from the repository whose name it was provided with");
                    Assert.AreEqual(fileVersion, givenFileMetadata.FileVersion, "Expected the SUT to request the file version from the repository that it was provided with");
                    Assert.AreEqual(cancellationToken, givenCancellationToken, "Expected the same cancellation token to propagate between service interfaces");

                    await givenStream.WriteAsync(fileBuffer, cancellationToken);
                    await givenStream.FlushAsync(cancellationToken);

                    fileRepositoryInvoked = true;
                }).
                Returns(Task.FromResult(fileWriteDetails));

            var fileMetadataProvider = new Moq.Mock<IUserFileMetadataProvider>();

            fileMetadataProvider.Setup(x => x.GetForFileAsync(Moq.It.IsAny<File>(), Moq.It.IsAny<AuthenticatedUser>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(fileMetadata));

            services.AddScoped(sp => fileMetadataProvider.Object);

            var file = File.With(fileName, fileVersion);

            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default);

            var getFileWopiRequest = GetFileWopiRequestHandler.With(authenticatedUser, file);

            httpContext.RequestServices = services.BuildServiceProvider();

            await getFileWopiRequest.HandleAsync(httpContext, cancellationToken);

            Assert.IsTrue(fileRepositoryInvoked, "Expected the SUT to defer to the file repository with the correct parameters");

            Assert.AreEqual(fileBuffer.Length, responseBodyStream.Length, "All bytes in the file should be written to the target stream");

            Assert.IsTrue(httpContext.Response.Headers.ContainsKey("X-WOPI-ItemVersion"), "Expected the X-WOPI-ItemVersion header to have been written to the response");

            Assert.IsNotNull(httpContext.Response.Headers["X-WOPI-ItemVersion"], "Expected the X-WOPI-ItemVersion header in the response to not be null");
        }
    }
}
