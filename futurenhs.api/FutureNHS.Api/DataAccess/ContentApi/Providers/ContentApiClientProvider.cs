using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;

namespace FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProvider
{
    public sealed class ContentApiClientProvider : IContentApiClientProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContentApiClientProvider> _logger;

        public ContentApiClientProvider(IConfiguration configuration, ILogger<ContentApiClientProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<T> SendRequestAsync<T>(HttpMethod method, string requestUrl, JsonContent? json = null)
        {
            try
            {
                // REMOVES SSL - FOR DEVELOPMENT ONLY
                var httpClientHandler = new HttpClientHandler();
                //httpClientHandler.ServerCertificateCustomValidationCallback = (
                //    message,
                //    cert,
                //    chain,
                //    sslPolicyErrors) =>
                //{
                //    return true;
                //};

                var baseUrl = _configuration.GetValue<Uri>("ContentApi:ContentApiUrl");
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
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Error connecting to content Api Client - {ex.Message}");
            }
            return default(T);
        }
    }
}

