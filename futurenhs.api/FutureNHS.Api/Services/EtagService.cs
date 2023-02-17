using FutureNHS.Api.Services.Interfaces;
using Microsoft.Net.Http.Headers;

namespace FutureNHS.Api.Services
{
    public sealed class EtagService : IEtagService
    {
        private readonly HttpContext _httpContext;

        public EtagService(IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(httpContextAccessor);
            ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

            _httpContext = httpContextAccessor.HttpContext;
        }

        public byte[] GetIfMatch()
        {
            byte[] IfMatch;
            try
            {
                IfMatch = Convert.FromBase64String(_httpContext.Request.Headers[HeaderNames.IfMatch].ToString());
            }
            catch (FormatException ex)
            {
                throw new BadHttpRequestException("If-Match header is not a valid Base-64 string ", ex);
            }

            return IfMatch.Length > 0 ? IfMatch : throw new BadHttpRequestException("If-Match header not set");
        }

        public byte[] GetIfNoneMatch()
        {
            byte[] IfNoneMatch;
            try
            {
                IfNoneMatch = Convert.FromBase64String(_httpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString());
            }
            catch (FormatException ex)
            {
                throw new BadHttpRequestException("If-Match header is not a valid Base-64 string ", ex);
            }
            return IfNoneMatch.Length > 0 ? IfNoneMatch : throw new BadHttpRequestException("If-None-Match header not set");
        }
    }
}
