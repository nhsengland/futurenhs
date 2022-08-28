using FutureNHS.WOPIHost;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS.WOPIHost.WOPIRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests.WOPIRequests
{
    [TestClass]
    public sealed class CheckFileInfoWopiRequestHandlerTests
    {
        [TestMethod]
        [DataRow("file-title1", "file-description1", "group1", "owner1", "Excel-Spreadsheet.xlsx", ".xlsx", ulong.MaxValue, "hash")]
        [DataRow("file-title2", "file-description2", "group2", "owner2", "Image-File.jpg", ".jpg", ulong.MaxValue, "hash")]
        [DataRow("file-title3", "file-description3", "group3", "owner3", "OpenDocument-Text-File.odt", ".odt", ulong.MaxValue, "hash")]
        [DataRow("file-title4", "file-description4", "group4", "owner4", "Portable-Document-Format-File.pdf", ".pdf", ulong.MaxValue, "hash")]
        [DataRow("file-title5", "file-description5", "group5", "owner5", "PowerPoint-Presentation.pptx", ".pptx", ulong.MaxValue, "hash")]
        [DataRow("file-title6", "file-description6", "group6", "owner6", "Text-File.txt", ".txt", ulong.MaxValue, "hash")]
        [DataRow("file-title7", "file-description7", "group7", "owner7", "Word-Document.docx", ".docx", ulong.MaxValue, "hash")]
        public async Task HandleAsync_FormsWOPICompliantResponseUsingFileMetadataAndUserContextAndFeatures(string title, string description, string groupName, string owner, string fileName, string extension, ulong sizeInBytes, string contentHash)
        {
            var cancellationToken = new CancellationToken();

            var services = new ServiceCollection();

            var fileMetadataRepository = new Moq.Mock<IUserFileMetadataProvider>();

            var fileRepositoryInvoked = false;

            services.AddScoped(sp => fileMetadataRepository.Object);

            var httpContext = new DefaultHttpContext {
                RequestServices = services.BuildServiceProvider()
            };

            using var responseBodyStream = new MemoryStream();

            httpContext.Response.Body = responseBodyStream;

            var authenticatedUser = new AuthenticatedUser(Guid.NewGuid(), default);

            var fileId = Guid.NewGuid();
            var fileVersion = Guid.NewGuid().ToString();

            var file = FutureNHS.WOPIHost.File.With(fileName, fileVersion);

            var fileMetadata = new UserFileMetadata() {
                FileId = fileId,
                Title = title,
                Description = description,
                GroupName = groupName,
                FileVersion = fileVersion,
                OwnerUserName = owner,
                Name = fileName,
                Extension = extension,
                BlobName = fileId.ToString() + extension,
                SizeInBytes = sizeInBytes,
                LastWriteTimeUtc = DateTimeOffset.UtcNow,
                ContentHash = Convert.FromBase64String("aGFzaA == "),
                UserHasViewPermission = true,
                UserHasEditPermission = false
                };

            fileMetadataRepository.
                Setup(x => x.GetForFileAsync(Moq.It.IsAny<FutureNHS.WOPIHost.File>(), Moq.It.IsAny<AuthenticatedUser>(), Moq.It.IsAny<CancellationToken>())).
                Callback((FutureNHS.WOPIHost.File givenFile, AuthenticatedUser givenAuthenticatedUser, CancellationToken givenCancellationToken) => {

                    Assert.IsFalse(givenFile.IsEmpty);

                    Assert.IsFalse(givenCancellationToken.IsCancellationRequested, "Expected the cancellation token to not be cancelled");

                    Assert.AreEqual(file.Id, givenFile.Id, "Expected the SUT to request the file from the repository whose id it was provided with");
                    Assert.AreEqual(fileName, givenFile.Name, "Expected the SUT to request the file from the repository whose name it was provided with");
                    Assert.AreEqual(fileVersion, givenFile.Version, "Expected the SUT to request the file version from the repository that it was provided with");
                    Assert.AreEqual(authenticatedUser, givenAuthenticatedUser, "Expected the SUT to pass to the repository the authenticated user that it was provided with");
                    Assert.AreEqual(cancellationToken, givenCancellationToken, "Expected the same cancellation token to propagate between service interfaces");

                    fileRepositoryInvoked = true;
                }).
                Returns(Task.FromResult(fileMetadata));

            var features = new Features();


            var checkFileInfoWopiRequestHandler = CheckFileInfoWopiRequestHandler.With(authenticatedUser, file, features);

            await checkFileInfoWopiRequestHandler.HandleAsync(httpContext, cancellationToken);

            Assert.IsTrue(fileRepositoryInvoked);

            Assert.AreEqual("application/json; charset=utf-8", httpContext.Response.ContentType);

            Assert.AreSame(responseBodyStream, httpContext.Response.Body);

            responseBodyStream.Position = 0;

            dynamic responseBody = await JsonSerializer.DeserializeAsync<ExpandoObject>(responseBodyStream, cancellationToken: cancellationToken);

            Assert.IsNotNull(responseBody);

            Assert.AreEqual(fileMetadata.Title, ((JsonElement)(responseBody.BaseFileName)).GetString());
            Assert.AreEqual(fileMetadata.FileVersion, ((JsonElement)(responseBody.Version)).GetString());
            Assert.AreEqual(fileMetadata.OwnerUserName, ((JsonElement)(responseBody.OwnerId)).GetString());
            Assert.AreEqual(fileMetadata.Extension, ((JsonElement)(responseBody.FileExtension)).GetString());
            Assert.AreEqual(fileMetadata.SizeInBytes, ((JsonElement)(responseBody.Size)).GetUInt64());
            Assert.AreEqual(fileMetadata.LastWriteTimeUtc.ToIso8601(), ((JsonElement)(responseBody.LastModifiedTime)).GetString());

            Assert.AreEqual(FutureNHS.WOPIHost.File.FILENAME_MAXIMUM_LENGTH, ((JsonElement)(responseBody.FileNameMaxLength)).GetInt32());
        }

        // TODO - Tests needed to verify correct propagation of data from owner context and feature flag configuration
    }
}
