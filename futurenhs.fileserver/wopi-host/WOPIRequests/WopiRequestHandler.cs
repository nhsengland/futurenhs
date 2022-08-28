using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost.WOPIRequests
{
    public abstract class WopiRequestHandler
    {
        internal static readonly WopiRequestHandler Empty = new EmptyWopiRequest();

        protected WopiRequestHandler() { }

        protected WopiRequestHandler(bool demandsProof)
        {
            DemandsProof = demandsProof;
        }

        public bool? DemandsProof { get; }

        internal bool IsEmpty => ReferenceEquals(this, WopiRequestHandler.Empty);

        internal async Task HandleAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            if (IsEmpty) throw new InvalidOperationException("An empty wopi request cannot handle an http context.  Check IsEmpty before invoking this method");

            cancellationToken.ThrowIfCancellationRequested();

            var responseStatusCode = await HandleAsyncImpl(httpContext, cancellationToken);

            if (httpContext.Response.HasStarted) return;

            httpContext.Response.StatusCode = responseStatusCode;
        }


        protected abstract Task<int> HandleAsyncImpl(HttpContext httpContext, CancellationToken cancellationToken);

        protected virtual void WriteNotFoundResponse(HttpContext httpContent)
        {
            httpContent.Response.StatusCode = StatusCodes.Status404NotFound;
        }

        protected virtual void WritePermissionDeniedResponse(HttpContext httpContent)
        {
            httpContent.Response.StatusCode = StatusCodes.Status403Forbidden;
        }


        /// <summary>
        /// Helper class used to capture a failure to identify/verify a potential WOPI request such that we don't have to 
        /// propagate nulls throughout the code.  Could equally be switched out for a Maybe type upon review
        /// </summary>
        private sealed class EmptyWopiRequest : WopiRequestHandler
        {
            internal EmptyWopiRequest() { }

            protected override Task<int> HandleAsyncImpl(HttpContext context, CancellationToken cancellationToken) => throw new NotImplementedException();
        }
    }
}
