using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost.WOPIRequests
{
    internal sealed class PostFileWopiRequestHandler
        : WopiRequestHandler
    {
        private readonly string _fileId;

        private PostFileWopiRequestHandler(string fileId)
            : base(true)
        {
            if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentNullException(nameof(fileId));

            _fileId = fileId;
        }

        internal static PostFileWopiRequestHandler With(string fileId) => new(fileId);

        protected override Task<int> HandleAsyncImpl(HttpContext httpContext, CancellationToken cancellationToken)
        {
            // NB - WHEN WRITING TO AZURE BLOB ENSURE WE SET THE CONTENT DISPOSITION HEADER TO THE NAME OF THE FILE THAT WAS UPLOADED
            //      THINK ABOUT WHETHER A USER CAN CHANGE THE FILENAME AFTER IT HAS BEEN UPLOADED.   IF SO, MIGHT
            //      CAUSE US PROBLEMS UNLESS BLOB STORAGE LETS US CHANGE IT
            //      x-ms-blob-content-disposition : https://docs.microsoft.com/en-us/rest/api/storageservices/put-blob

            // POST /wopi/files/(file_id)/content 

            // TODO - This is where we would update the file in our storage repository
            //        taking care of permissions, locking and versioning along the way 

            return Task.FromResult(StatusCodes.Status501NotImplemented);
        }
    }
}
