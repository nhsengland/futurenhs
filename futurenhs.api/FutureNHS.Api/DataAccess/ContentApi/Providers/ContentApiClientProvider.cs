using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProvider
{
    public sealed class ContentApiClientProvider : IContentApiClientProvider
    {
        private readonly IConfiguration _configuration;

        public ContentApiClientProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<T> SendRequestAsync<T>(HttpMethod method, string requestUrl, JsonContent? json = null)
        {
            // REMOVES SSL - FOR DEVELOPMENT ONLY
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (
                message,
                cert,
                chain,
                sslPolicyErrors) =>
            {
                return true;
            };

            var baseUrl = _configuration.GetValue<Uri>("ContentApi:ContentApiPUrl");
            var absoluteUrl = new Uri(baseUrl + requestUrl);

            using (var client = new HttpClient(httpClientHandler))
            {
                using var httpRequestMessage = new HttpRequestMessage(method, absoluteUrl)
                {
                    Content = json
                };

                var response = await client
                    .SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<T>();
                    return result;
                }

                return default(T);
            };
        }
    }
}

