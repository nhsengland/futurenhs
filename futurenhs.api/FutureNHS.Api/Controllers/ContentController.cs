using FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Models.Content;
using FutureNHS.Api.Models.Content.Blocks;
using FutureNHS.Api.Models.Content.Requests;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private readonly IImageDataProvider _imageDataProvider;
        private readonly IContentService _contentService;
        private readonly IContentApiRequestHandler _contentDataProvider;

        public ContentController(ILogger<ContentController> logger, IImageDataProvider imageDataProvider, IContentApiRequestHandler contentDataProvider, IContentService contentService)
        {
            _logger = logger;
            _imageDataProvider = imageDataProvider;
            _contentDataProvider = contentDataProvider;
            _contentService = contentService;
        }

        [HttpGet]
        [Route("image/{imageId}")]
        public async Task<IActionResult> GetImageAsync(Guid imageId)
        {
            var image = await _imageDataProvider.GetImageAsync(imageId);
            return File(image.Data, image.MediaType);
        }

        [HttpPost]
        [Route("page/{userId}/{groupId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> CreatePageAsync(Guid userId, Guid groupId, [FromBody] GeneralWebPageCreateRequest createRequest, CancellationToken cancellationToken)
        {
            var pageGuid = await _contentService.CreateContentAsync(userId, groupId, createRequest, cancellationToken);
            return new JsonResult(pageGuid);
        }

        [HttpGet]
        [Route("page/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModel>))]
        public async Task<IActionResult> GetPageAsync(Guid pageId)
        {
            var page = await _contentDataProvider.GetContentAsync(pageId);
            return new JsonResult(page);
        }

        [HttpPut]
        [Route("page/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> UpdatePageAsync(Guid? userId, Guid pageId, [FromBody] GeneralWebPageUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            var pageGuid = await _contentService.UpdateContentAsync(userId, pageId, updateRequest, cancellationToken);
            return new JsonResult(pageGuid);
        }

        [HttpDelete]
        [Route("page/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> DeletePageAsync(Guid? userId, Guid pageId, CancellationToken cancellationToken, int? contentLevel = null)
        {
            var pageGuid = await _contentService.DeleteContentAsync(userId, pageId, contentLevel, cancellationToken);
            return new JsonResult(pageGuid);
        }

        [HttpPost]
        [Route("page/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> PublishPageAsync(Guid pageId, CancellationToken cancellationToken)
        {
            var pageGuid = await _contentService.PublishContentAsync(pageId, cancellationToken);
            return Ok(pageGuid);
        }

        [HttpGet]
        [Route("block/{blockId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<BlockModel>))]
        public async Task<IActionResult> GetBlockAsync(Guid blockId)
        {
            var block = await _contentDataProvider.GetBlockAsync(blockId);
            return new JsonResult(block);
        }

        [HttpGet]
        [Route("block/{blockId:guid}/placeholder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetBlockPlaceholderValuesAsync(Guid blockId)
        {
            var block = await _contentDataProvider.GetBlockPlaceholderValuesAsync(blockId);
            return new JsonResult(block);
        }

        [HttpGet]
        [Route("block")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ContentModel>>))]
        public async Task<IActionResult> GetBlocksAsync()
        {
            var blocks = await _contentDataProvider.GetBlocksAsync();
            return new JsonResult(blocks);
        }

        [HttpGet]
        [Route("template/{templateId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModel>))]
        public async Task<IActionResult> GetTemplateAsync(Guid templateId)
        {
            var template = await _contentDataProvider.GetTemplateAsync(templateId);
            return new JsonResult(template);
        }

        [HttpGet]
        [Route("template")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ContentModel>>))]
        public async Task<IActionResult> GetTemplatesAsync()
        {
            var templates = await _contentDataProvider.GetTemplatesAsync();
            return new JsonResult(templates);
        }

        [HttpGet]
        [Route("sitemap/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SitemapGroupItemModel>))]
        public async Task<IActionResult> GetSiteMapAsync(Guid pageId)
        {
            var sitemap = await _contentDataProvider.GetSiteMapAsync(pageId);
            return new JsonResult(sitemap);
        }
    }
}
