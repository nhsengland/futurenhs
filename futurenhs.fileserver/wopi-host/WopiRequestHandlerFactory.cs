using FutureNHS.WOPIHost.Configuration;
using FutureNHS.WOPIHost.WOPIRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    public interface IWopiRequestHandlerFactory
    {
        Task<WopiRequestHandler> CreateRequestHandlerAsync(HttpContext httpContext, CancellationToken cancellationToken);
    }

    internal sealed class WopiRequestHandlerFactory
        : IWopiRequestHandlerFactory
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IUserFileMetadataProvider _userFileMetadataProvider;
        private readonly Features _features;
        private readonly ILogger<WopiRequestHandlerFactory>? _logger;

        public WopiRequestHandlerFactory(IUserAuthenticationService userAuthenticationService, IUserFileMetadataProvider userFileMetadataProvider, IOptionsSnapshot<Features> features, ILogger<WopiRequestHandlerFactory>? logger = default)
        {
            _userAuthenticationService = userAuthenticationService ?? throw new ArgumentNullException(nameof(userAuthenticationService));
            _userFileMetadataProvider = userFileMetadataProvider   ?? throw new ArgumentNullException(nameof(userFileMetadataProvider));
            _features = features?.Value                            ?? throw new ArgumentNullException(nameof(features));

            _logger = logger;
        }


        async Task<WopiRequestHandler> IWopiRequestHandlerFactory.CreateRequestHandlerAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpContext is null) throw new ArgumentNullException(nameof(httpContext));

            var httpRequest = httpContext.Request;

            var requestPath = httpRequest.Path;

            if (!requestPath.IsWopiPath()) return NotAConsumableWopiRequest("The request is not known to be a valid WOPI request path: '{RequestPath}'", requestPath.Value ?? "null");

            if (requestPath.IsHealthCheck()) return WopiRequestHandler.Empty;

            if (requestPath.IsCollaboraTestPage()) return WopiRequestHandler.Empty;

            if (!requestPath.IsWopiFilePath()) return NotAConsumableWopiRequest("Failed to identify WOPI request.  Endpoint '{RequestPath}' not supported", requestPath);

            return await IdentifyFileRequestAsync(httpContext, cancellationToken);           
        }

        private async Task<WopiRequestHandler> ForbiddenWopiRequestAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            _logger?.LogError("The request to {RequestPath} could not be authorised.   No valid auth cookie or access_token was found", httpContext.Request.Path);

            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

            var msg = $"The request could not be authorised.   Ensure the Cookie header contains an auth token or a valid access_token is presented with the request";

            await httpContext.Response.WriteAsync(msg, cancellationToken);

            return WopiRequestHandler.Empty;
        }

        private async Task<WopiRequestHandler> IdentifyFileRequestAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            var httpRequest = httpContext.Request;

            var requestPath = httpRequest.Path.Value;

            if (string.IsNullOrWhiteSpace(requestPath)) return NotAConsumableWopiRequest("Thre is no request path");
 
            var fileSegmentOfRequestPath = requestPath[ExtensionMethods.WOPI_FILES_PATH_SEGMENT.Length..]?.Trim();

            var requestMethod = httpRequest.Method;

            _logger?.LogTrace("Extracted file segment from request path: '{PathSegment}' for method '{RequestMethod}'", fileSegmentOfRequestPath ?? "null", requestMethod);
            
            if (string.IsNullOrWhiteSpace(fileSegmentOfRequestPath)) return NotAConsumableWopiRequest("The request path is not correctly formed");

            Debug.Assert(fileSegmentOfRequestPath.StartsWith('/'));

            fileSegmentOfRequestPath = fileSegmentOfRequestPath[1..];

            if (fileSegmentOfRequestPath.EndsWith("/authorise-user"))
            {
                return await ConfigureAuthoriseUserRequestHandlerAsync(httpContext, requestPath, requestMethod, fileSegmentOfRequestPath, httpRequest.Query, cancellationToken);
            }
            else if (fileSegmentOfRequestPath.EndsWith("/contents"))
            {
                return await ConfigureFileContentRequestHandlerAsync(httpContext, requestMethod, requestPath, fileSegmentOfRequestPath, cancellationToken);
            }
          
            return await ConfigureCheckFileInfoRequestHandlerAsync(httpContext, requestPath, requestMethod, fileSegmentOfRequestPath, cancellationToken);            
        }

        private async Task<WopiRequestHandler> ConfigureFileContentRequestHandlerAsync(HttpContext httpContext, string requestMethod, string requestPath, string fileSegmentOfRequestPath, CancellationToken cancellationToken)
        {
            var fileId = fileSegmentOfRequestPath[..^"/contents".Length]?.Trim();

            _logger?.LogTrace("Identified 'contents' request.  File Id extracted from url is: '{FileId}'", fileId);

            if (string.IsNullOrWhiteSpace(fileId)) return NotAConsumableWopiRequest("The file id is missing from the /contents request");

            // NB - Collabora have not implemented support for the X-WOPI-ItemVersion header and so the Version field set in the 
            //      CheckFileInfo response does not flow through to those endpoints where it is optional - eg GetFile.
            //      This unfortunately means we have to do some crazy workaround using the fileId, and thus use that to derive 
            //      the relevant metadata needed for us to operate correctly.  Hopefully this will prove to be just a temporary
            //      workaround until Collabora complete the necessary work or we can submit a PR to them

            var httpRequest = httpContext.Request;

            var fileVersion = httpRequest.Headers["X-WOPI-ItemVersion"].FirstOrDefault();

            var file = File.FromId(fileId, fileVersion);

            var authenticatedUser = await _userAuthenticationService.GetForFileContextAsync(httpContext, file, cancellationToken);

            if (authenticatedUser is null) return await ForbiddenWopiRequestAsync(httpContext, cancellationToken);

            if (authenticatedUser.FileMetadata is null) return NotAConsumableWopiRequest("The user file metadata could not be verified.  Likely cause is the request does not contain a valid access_token but does have an authentication cookie");

            if (file != authenticatedUser.FileMetadata.AsFile()) return NotAConsumableWopiRequest("The file associated with the request does not match the file that the user has been authorised to access");

            if (0 == string.Compare("GET", requestMethod, StringComparison.OrdinalIgnoreCase))
            {
                _logger?.LogTrace("Identified this to be a WOPI 'Get File' request");

                return GetFileWopiRequestHandler.With(authenticatedUser, file);
            }
            else if (0 == string.Compare("POST", requestMethod, StringComparison.OrdinalIgnoreCase))
            {
                _logger?.LogTrace("Identified this to be a WOPI 'Save File' request");

                Debug.Assert(!string.IsNullOrWhiteSpace(file.Name));

                return PostFileWopiRequestHandler.With(file.Name);
            }

            return NotAConsumableWopiRequest("The request method '{RequestMethod}' is not supported for path '{RequestPath}'", requestMethod, requestPath ?? "null");
        }

        private async Task<WopiRequestHandler> ConfigureAuthoriseUserRequestHandlerAsync(HttpContext httpContext, string requestPath, string requestMethod, string fileSegmentOfRequestPath, IQueryCollection query, CancellationToken cancellationToken)
        {
            var fileId = fileSegmentOfRequestPath.Replace("/authorise-user", string.Empty);

            _logger?.LogTrace("File Id extracted from url is: '{FileId}'.  Attempting to identify permission type", fileId ?? "null");

            if (string.IsNullOrWhiteSpace(fileId)) return NotAConsumableWopiRequest("The file id is missing from the /authorise-user request");

            if (0 == string.Compare("POST", requestMethod, StringComparison.OrdinalIgnoreCase))
            {
                // This isn't actually part of the WOPI specification; more a customisation we're making to get the endpoint for the WOPI host
                // from where the file can be viewed/edited, and an access_token it can pass back to us when requesting the same

                _logger?.LogTrace("Identified this to be a POST /authorise-user' request");

                var file = File.FromId(fileId);

                var authenticatedUser = await _userAuthenticationService.GetForFileContextAsync(httpContext, file, cancellationToken);

                if (authenticatedUser is null) return await ForbiddenWopiRequestAsync(httpContext, cancellationToken);

                // If the file metadata is known, it is because the access_token has been provided to this request.  Check it is 
                // tied to the same file that we need a new token for, else consider the request malformed.
                // If it is the same file, grab the metadata again just to verify user file permissions haven't changed and the file
                // is still active on the site

                if (authenticatedUser.FileMetadata is not null && authenticatedUser.FileMetadata.AsFile() != file)
                {
                    _logger?.LogCritical("An access_token has been provided that was verified to be valid, however it is not for the file that the requestor is trying to authorise.  This could be an attempt to hijack credentials");

                    return NotAConsumableWopiRequest("Only supply an access_token to authenticate requests specific to the previously authorised file");
                }

                var userFileMetadata = await _userFileMetadataProvider.GetForFileAsync(file, authenticatedUser, cancellationToken);

                if (userFileMetadata is null) return NotAConsumableWopiRequest("The user file metadata could not be retrieved");

                var permission = 0 == string.Compare(query["permission"], "edit", StringComparison.OrdinalIgnoreCase)
                    ? FileAccessPermission.Edit
                    : FileAccessPermission.View;

                _logger?.LogTrace("Permission type extracted from url query is: '{Permission}'.  Attempting to identity request type", permission);

                authenticatedUser = authenticatedUser with { FileMetadata = userFileMetadata };

                return AuthoriseUserRequestHandler.With(authenticatedUser, permission, file);
            }

            return NotAConsumableWopiRequest("The request method '{RequestMethod}' is not supported for path '{RequestPath}", requestMethod, requestPath);
        }

        private async Task<WopiRequestHandler> ConfigureCheckFileInfoRequestHandlerAsync(HttpContext httpContext, string requestPath, string requestMethod, string fileSegmentOfRequestPath, CancellationToken cancellationToken)
        {
            var fileId = fileSegmentOfRequestPath;

            _logger?.LogTrace("File Id extracted from url is: '{FileId}'", fileId);

            if (0 == string.Compare("GET", requestMethod, StringComparison.OrdinalIgnoreCase))
            {
                _logger?.LogTrace("Identified this is a WOPI 'Check File Info' request");

                var file = (File)fileId;

                var authenticatedUser = await _userAuthenticationService.GetForFileContextAsync(httpContext, file, cancellationToken);

                if (authenticatedUser is null) return await ForbiddenWopiRequestAsync(httpContext, cancellationToken);

                return CheckFileInfoWopiRequestHandler.With(authenticatedUser, file, _features);
            }

            return NotAConsumableWopiRequest("The request method '{RequestMethod}' is not supported for path '{RequestPath}", requestMethod, requestPath);
        }

#pragma warning disable CA2254 // Template should be a static expression
        private WopiRequestHandler NotAConsumableWopiRequest(string message, params object[] args) 
        {
            _logger?.LogWarning(message, args);
            
            return WopiRequestHandler.Empty;
        }
#pragma warning restore CA2254 // Template should be a static expression

    }
}
