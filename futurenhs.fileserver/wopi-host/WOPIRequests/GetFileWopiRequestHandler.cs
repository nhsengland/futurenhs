using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost.WOPIRequests
{
    internal sealed class GetFileWopiRequestHandler 
        : WopiRequestHandler
    {
        private readonly File _file;
        private readonly AuthenticatedUser _authenticatedUser;

        private GetFileWopiRequestHandler(AuthenticatedUser authenticatedUser, File file) 
            : base(true) 
        {
            if (authenticatedUser is null) throw new ArgumentNullException(nameof(authenticatedUser));
            if (file.IsEmpty) throw new ArgumentNullException(nameof(file));

            _file = file;
            _authenticatedUser = authenticatedUser;
        }

        internal static GetFileWopiRequestHandler With(AuthenticatedUser authenticatedUser, File file) => new(authenticatedUser, file);

        protected override async Task<int> HandleAsyncImpl(HttpContext httpContext, CancellationToken cancellationToken)
        {
            // GET /wopi/files/(file_id)/content 

            // TODO - Is it possible to redirect the WOPI file content request so that the WOPI client pulls the file down 
            //        directly from blob storage rather than have us acting as a proxy?  
            //        Tried httpResponse.Redirect("http://127.0.0.1:44355/redirected", permanent: false); locally without any 
            //        success (no callback) so need to investigate Collabora codebase to see if it can handle the redirect response
            //        as initial testing suggests it might not be doing so.
            //        Worth chasing up as potentially a solid win for us, as well as enabling us to reuse the download redirect
            //        code used by the main site

            var httpResponse = httpContext.Response;

            httpResponse.Headers.Add("X-WOPI-ItemVersion", _file.Version);

            var responseStream = httpResponse.Body;

            // TODO - Clarify this is the right time to start sending the body given it also locks headers for modification etc.
            //        Unsure yet whether we need to include headers with information pulled from blob storage or the file metadata 
            //        held in the database.  Going to start it before we pull from blob storage on the assumption (to be tested) 
            //        that this means for larger files we won't have to reserve enough memory/disk space to hold the complete file
            //        on this server

            var fileMetadataProvider = httpContext.RequestServices.GetRequiredService<IUserFileMetadataProvider>();

            var fileMetadata = await fileMetadataProvider.GetForFileAsync(_file, _authenticatedUser, cancellationToken);

            if (fileMetadata is null) return StatusCodes.Status404NotFound;
            if (!fileMetadata.UserHasViewPermission) return StatusCodes.Status403Forbidden;

            await httpResponse.StartAsync(cancellationToken);

            // This next bit is a little tricky because any validation we do after the call to write to the response stream still means 
            // the client might have access to the full file already.  In other words, not much we can assure about what happens next!

            var fileContentMetadataRepository = httpContext.RequestServices.GetRequiredService<IFileContentMetadataRepository>();

            var fileContentMetadata = await fileContentMetadataRepository.GetDetailsAndPutContentIntoStreamAsync(fileMetadata, responseStream, cancellationToken);

            if (fileContentMetadata.IsEmpty) throw new ApplicationException("Unable to pull the content for the requested file version");

            if (!string.Equals(fileContentMetadata.ContentVersion, _file.Version, StringComparison.OrdinalIgnoreCase)) throw new ApplicationException("The blob store client returned a version of the blob that does not match the version requested");

            // Done reading, so make sure we are done writing too

            await responseStream.FlushAsync(cancellationToken);

            return StatusCodes.Status200OK;
        }
    }
}
