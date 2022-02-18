using AspNetCore.Authentication.ApiKey;

namespace FutureNHS.Api.Authorization
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly string _apiKey;
        private readonly string _owner;
        private readonly ILogger<IApiKeyRepository> _logger;
        public ApiKeyRepository(string apiKey, string owner, ILogger<IApiKeyRepository> logger)
        {
            _apiKey = apiKey;
            _owner = owner;
            _logger = logger;
        }
        public Task<IApiKey> GetApiKeyAsync(string key)
        {
            var apiKey = new ApiKey(_apiKey, _owner);
            return Task.FromResult<IApiKey>(apiKey);
        }
    }
}
