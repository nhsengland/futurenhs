using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Content.Responses;
using FutureNHS.Api.DataAccess.Models.Requests;
using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;

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
        public Task<ApiResponse<string>> CreateContentAsync(ContentDto content, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Post, "api/content/create", JsonContent.Create(content));
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> UpdateContentAsync(Guid contentId, GeneralWebPageUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Put, $"api/content/{contentId}", JsonContent.Create(updateRequest));
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
