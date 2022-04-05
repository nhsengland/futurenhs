using FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;
using FutureNHS.Api.Models.Content;

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
        public Task<ApiResponse<ContentModel>> GetBlockAsync(Guid blockId)
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<ContentModel>>(HttpMethod.Get, $"api/block/{blockId}");
        }

        /// <inheritdoc />
        public Task<ApiResponse<IEnumerable<ContentModel>>> GetBlocksAsync()
        {
            return _contentApiClientProvider.SendRequestAsync<ApiResponse<IEnumerable<ContentModel>>>(HttpMethod.Get, "api/block");
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
