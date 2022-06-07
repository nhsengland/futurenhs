using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models.Content.Requests;
using FutureNHS.Api.DataAccess.Models.Content.Responses;
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
        public Task<ApiResponse<string>> CreatePageAsync(PageDto content, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Post, "api/page", JsonContent.Create(content));
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> CreateBlockAsync(BlockCreateRequest createRequest, CancellationToken cancellationToken)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<string>>(HttpMethod.Post, $"api/block", JsonContent.Create(createRequest));
        }

        /// <inheritdoc />
        public Task<ApiResponse<string>> UpdatePageAsync(Guid contentId, GeneralWebPageUpdateRequest pageModel, CancellationToken cancellationToken)
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
