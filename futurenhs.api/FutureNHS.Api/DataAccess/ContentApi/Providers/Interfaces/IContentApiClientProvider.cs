namespace FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces
{
    public interface IContentApiClientProvider
    {
        /// <summary>
        /// Sends the request asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="jsonContent">Content of the json.</param>
        /// <returns></returns>
        Task<T> SendRequestAsync<T>(HttpMethod method, string requestUrl, JsonContent? jsonContent = null);
    }
}
