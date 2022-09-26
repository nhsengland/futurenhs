using FutureNHS.WOPIHost.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost.WOPIRequests
{
    internal sealed class AuthoriseUserRequestHandler
        : WopiRequestHandler
    {

        private readonly FileAccessPermission _fileAccessPermission;
        private readonly AuthenticatedUser _authenticatedUser;
        private readonly File _file;

        private AuthoriseUserRequestHandler(AuthenticatedUser authenticatedUser, FileAccessPermission fileAccessPermission, File file)
            : base(false)
        {
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));
            if (file.IsEmpty) throw new ArgumentOutOfRangeException(nameof(file));

            if (!Enum.IsDefined(typeof(FileAccessPermission), fileAccessPermission))
            {
                fileAccessPermission = FileAccessPermission.View;
            }

            _authenticatedUser = authenticatedUser;
            _file = file;
            _fileAccessPermission = fileAccessPermission;
        }

        public static AuthoriseUserRequestHandler With(AuthenticatedUser authenticatedUser, FileAccessPermission permission, File file) => new (authenticatedUser, permission, file);

        protected override async Task<int> HandleAsyncImpl(HttpContext httpContext, CancellationToken cancellationToken)
        {
            // Try to get the discovery document from the WOPI client server (Collabora).  This tells us what file types are supported, and the
            // public endpoint that it hosts for it (that the browser needs to post back to in order to get said file)

            var wopiDiscoveryDocumentFactory = httpContext.RequestServices.GetRequiredService<IWopiDiscoveryDocumentFactory>();

            var wopiDiscoveryDocument = await wopiDiscoveryDocumentFactory.CreateDocumentAsync(cancellationToken);

            if (wopiDiscoveryDocument.IsEmpty) throw new ApplicationException("Unable to obtain the WOPI Discovery Document - most likely cause is the remote WOPI client is either unavailable or returned a non-success status code");

            Debug.Assert(!string.IsNullOrWhiteSpace(_file.Id));

            var userFileMetadata = _authenticatedUser.FileMetadata;

            if (userFileMetadata is null) return StatusCodes.Status404NotFound;
            if (!userFileMetadata.UserHasViewPermission) return StatusCodes.Status403Forbidden;
            if (FileAccessPermission.Edit == _fileAccessPermission && !userFileMetadata.UserHasEditPermission) return StatusCodes.Status403Forbidden;

            var fileExtension = userFileMetadata.Extension;

            if (string.IsNullOrWhiteSpace(fileExtension)) throw new ApplicationException($"The file extension for the file with id '{_file.Id}' is not stored in it's metadata");

            var wopiConfiguration = httpContext.RequestServices.GetRequiredService<IOptionsSnapshot<WopiConfiguration>>().Value;

            var hostFilesUrl = wopiConfiguration.HostFilesUrl;

            if (hostFilesUrl is null) throw new ApplicationException("Unable to determine the HostFilesUrl from the application configuration.  Entry is null.");
            if (!hostFilesUrl.IsAbsoluteUri) throw new ApplicationException($"Unable to determine the HostFilesUrl from the application configuration.  The entry is not a well formed absolute URL '{hostFilesUrl}'.");

            var fileAction = _fileAccessPermission == FileAccessPermission.View ? "view" : "edit"; // edit | view | etc (see comments in discoveryDoc.GetEndpointForAsync)

            var wopiHostFileEndpointUrl = new Uri(System.IO.Path.Combine(hostFilesUrl.AbsoluteUri, _file.Id), UriKind.Absolute);

            var wopiClientEndpointForFileExtension = wopiDiscoveryDocument.GetEndpointForFileExtension(fileExtension, fileAction, wopiHostFileEndpointUrl);
           
            if (wopiClientEndpointForFileExtension is null || !wopiClientEndpointForFileExtension.IsAbsoluteUri) throw new ApplicationException($"The WOPI Client's endpoint for the requested file extension '{fileExtension}' could not be determined.  Ensure the file type is supported");

            Debug.Assert(_authenticatedUser?.Id is not null);

            var userAuthenticationService = httpContext.RequestServices.GetRequiredService<IUserAuthenticationService>();

            var accessToken = await userAuthenticationService.GenerateAccessToken(_authenticatedUser, _file, _fileAccessPermission, cancellationToken);

            var responseBody = new Dictionary<string, string>(2);

            responseBody["accessToken"] = accessToken.Id.ToString();
            responseBody["wopiClientUrlForFile"] = wopiClientEndpointForFileExtension.AbsoluteUri;

            await httpContext.Response.WriteAsJsonAsync(responseBody, cancellationToken);

            return StatusCodes.Status200OK;
        }
    }
}
