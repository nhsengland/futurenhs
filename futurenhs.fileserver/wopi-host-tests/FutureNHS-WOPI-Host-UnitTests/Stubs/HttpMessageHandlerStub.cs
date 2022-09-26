using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS_WOPI_Host_UnitTests.Stubs
{
    internal sealed class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _handler;

        internal HttpMessageHandlerStub(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler) { _handler = handler; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _handler(request, cancellationToken);

            return Task.FromResult(response);
        }
    }
}
