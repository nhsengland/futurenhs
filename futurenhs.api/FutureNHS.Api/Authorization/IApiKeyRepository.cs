using AspNetCore.Authentication.ApiKey;

namespace FutureNHS.Api.Authorization
{
    public interface IApiKeyRepository
    {
        Task<IApiKey> GetApiKeyAsync(string key);
    }
}