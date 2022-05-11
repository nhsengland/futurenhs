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
        public Task<ApiResponse<string>> CreatePageAsync(CreatePageRequest createRequest, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Post, "api/page", JsonContent.Create(createRequest));
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> CreateBlockAsync(CreateBlockRequest createRequest, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Post, $"api/block", JsonContent.Create(createRequest));
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> UpdatePageAsync(Guid contentId, PageModel pageModel, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Put, $"api/page/{contentId}", JsonContent.Create(pageModel));
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

        public Task<ApiResponse<string>> DiscardDraftContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Delete, $"api/content/{contentId}/discard");
        }

        public Task<ApiResponse<string>> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Put, $"api/page/{userId}/{pageId}");
        }

        public Task<ApiResponse<string>> CheckPageEditStatusAsync(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Get, $"api/page/{pageId}/editStatus");
        }
    }
}
