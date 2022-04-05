using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Models.Content;
using FutureNHS.Api.Models.Content.Requests;

namespace FutureNHS.Api.DataAccess.Repositories.Write
{
    public class ContentCommand : IContentCommand
    {
        private readonly IContentApiClientProvider _contentApiClientProvider;
        private readonly ILogger<ContentCommand> _logger;

        public ContentCommand(ILogger<ContentCommand> logger, IContentApiClientProvider contentApiClientProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contentApiClientProvider = contentApiClientProvider;
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> CreateContentAsync(GeneralWebPageCreateRequest createRequest, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Post, "api/content/create", JsonContent.Create(createRequest));
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> UpdateContentAsync(Guid contentId, GeneralWebPageUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Put, "api/content/create", JsonContent.Create(updateRequest));
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> DeleteContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Delete, $"api/content/{contentId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> PublishContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Post, $"api/content/{contentId}/publish");
        }
    }
}
