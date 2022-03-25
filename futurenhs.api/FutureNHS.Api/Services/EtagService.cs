using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.Net.Http.Headers;

namespace FutureNHS.Api.Services
{
    public sealed class EtagService : IEtagService
    {
        private byte[] IfMatch;
        private byte[] IfNoneMatch;

        public EtagService(IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(httpContextAccessor);

            IfMatch = Convert.FromBase64String(httpContextAccessor.HttpContext.Request.Headers[HeaderNames.IfMatch].ToString());
            IfNoneMatch = Convert.FromBase64String(httpContextAccessor.HttpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString());
        }

        public byte[] GetIfMatch()
        {
            return IfMatch.Length > 0 ? IfMatch : throw new BadHttpRequestException("If-Match header not set");
        }

        public byte[] GetIfNoneMatch()
        {
            return IfNoneMatch.Length > 0 ? IfNoneMatch : throw new BadHttpRequestException("If-None-Match header not set");
        }
    }
}
