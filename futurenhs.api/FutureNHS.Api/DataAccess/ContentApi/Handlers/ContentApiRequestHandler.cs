using FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces;
using FutureNHS.Api.DataAccess.Models.Content;
using FutureNHS.Api.DataAccess.Models.Content.Responses;
using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;

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
        public Task<ApiResponse<ContentModelData>> GetContentAsync(Guid contentId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModelData>>(HttpMethod.Get, $"api/content/{contentId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<ContentModelData>> GetContentPublishedAsnyc(Guid contentId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModelData>>(HttpMethod.Get, $"api/content/{contentId}/published");
        }

        /// <inheritdoc />
        public Task<ApiResponse<ContentModelData>> GetContentDraftAsnyc(Guid contentId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModelData>>(HttpMethod.Get, $"api/content/{contentId}/draft");
        }

        /// <inheritdoc />
        public Task<ApiResponse<ContentModelData>> GetBlockAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModelData>>(HttpMethod.Get, $"api/block/{blockId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<ContentModelData>>> GetAllBlocksAsync()
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<ContentModelData>>>(HttpMethod.Get, "api/block");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<string>>> GetBlockContentPlaceholderValuesAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<string>>>(HttpMethod.Get, $"api/block/{blockId}/content/placeholder");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<string>>> GetBlockLabelsPlaceholderValuesAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<string>>>(HttpMethod.Get, $"api/block/{blockId}/labels/placeholder");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<string>>> GetBlockContentAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<string>>>(HttpMethod.Get, $"api/block/{blockId}/content");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<string>>> GetBlockLabelsAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<string>>>(HttpMethod.Get, $"api/block/{blockId}/labels");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<SitemapGroupItemModelData>>> GetSiteMapAsync(Guid contentId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<SitemapGroupItemModelData>>>(HttpMethod.Get, $"api/sitemap/{contentId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<ContentModelData>> GetTemplateAsync(Guid templateId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModelData>>(HttpMethod.Get, $"api/template/{templateId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<ContentModelData>>> GetTemplatesAsync()
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<ContentModelData>>>(HttpMethod.Get, "api/template");
        }
    }
}

