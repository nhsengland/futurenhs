using FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;
using FutureNHS.Api.Models.Content;
using FutureNHS.Api.Models.Content.Blocks;

namespace FutureNHS.Api.DataAccess.ContentApi.Handlers
{
    public class ContentApiRequestHandler : IContentApiRequestHandler
    {
        private readonly IContentApiClientProvider _contentApiClientProvider;

        public ContentApiRequestHandler(IContentApiClientProvider contentApiClientProvider)
        {
            _contentApiClientProvider = contentApiClientProvider;
        }

        /// <inheritdoc />
        public Task<ApiResponse<BlockModel>> GetBlockAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<BlockModel>>(HttpMethod.Get, $"api/block/{blockId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<string>>> GetBlockPlaceholderValuesAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<string>>>(HttpMethod.Get, $"api/block/{blockId}/placeholder");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<BlockModel>>> GetBlocksAsync()
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<BlockModel>>>(HttpMethod.Get, "api/block");
        }

        /// <inheritdoc />
        public Task<ApiResponse<ContentModel>> GetContentAsync(Guid contentId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModel>>(HttpMethod.Get, $"api/content/{contentId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<SitemapGroupItemModel>>> GetSiteMapAsync(Guid contentId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<SitemapGroupItemModel>>>(HttpMethod.Get, $"api/sitemap/{contentId}");
        }

        public Task<ApiResponse<ContentModel>> GetTemplateAsync(Guid templateId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModel>>(HttpMethod.Get, $"api/template/{templateId}");
        }

        public Task<ApiResponse<IEnumerable<ContentModel>>> GetTemplatesAsync()
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<ContentModel>>>(HttpMethod.Get, "api/template");
        }
    }
}
