using FutureNHS.WOPIHost.WOPIRequests;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests.Stubs
{
    internal sealed class WopiRequestHandlerStub : WopiRequestHandler
    {
        private readonly Func<HttpContext, CancellationToken, Task<int>> _handleAsyncImpl;
  
        internal WopiRequestHandlerStub(Func<HttpContext, CancellationToken, Task<int>> handleAsyncImpl)
            : base(true)
        {
            _handleAsyncImpl = handleAsyncImpl;
        }

        protected override Task<int> HandleAsyncImpl(HttpContext context, CancellationToken cancellationToken)
        {
            return _handleAsyncImpl?.Invoke(context, cancellationToken);
        }
    }
}
