using FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.Content;
using FutureNHS.Api.DataAccess.Models.Content.Requests;
using FutureNHS.Api.DataAccess.Models.Content.Responses;
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
        [Route("page/{userId:guid}/{groupId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> CreatePageAsync(Guid userId, Guid groupId, [FromBody] GeneralWebPageCreateRequest createRequest, CancellationToken cancellationToken)
        {
            var pageGuid = await _contentService.CreatePageAsync(userId, groupId, createRequest, cancellationToken);
            return new JsonResult(pageGuid);
        }

        [HttpGet]
        [Route("page/{pageId:guid}/published")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModelData>))]
        public async Task<IActionResult> GetPagePublishedAsync(Guid pageId)
        {
            var page = await _contentDataProvider.GetContentPublishedAsnyc(pageId);
            return new JsonResult(page);
        }

        [HttpGet]
        [Route("page/{pageId:guid}/draft")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModelData>))]
        public async Task<IActionResult> GetPageDraftAsync(Guid pageId)
        {
            var page = await _contentDataProvider.GetContentDraftAsnyc(pageId);
            return new JsonResult(page);
        }

        [HttpPut]
        [Route("page/{userId:guid}/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> UpdatePageAsync(Guid userId, Guid pageId, [FromBody] GeneralWebPageUpdateRequest updateRequest, CancellationToken cancellationToken)
        {
            if (updateRequest.Blocks != null)
                foreach (var block in updateRequest.Blocks)
                {
                    if (block.Item.ContentType == "textBlock" && block.Content.ContainsKey("blocks"))
                    {
                        block.Content.Remove("blocks");
                    }
                }

            var pageGuid = await _contentService.UpdatePageAsync(userId, pageId, updateRequest, cancellationToken);
            return new JsonResult(pageGuid);
        }

        [HttpDelete]
        [Route("page/{userId:guid}/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> DeletePageAsync(Guid userId, Guid pageId, CancellationToken cancellationToken, int? contentLevel = null)
        {
            var response = await _contentService.DeleteContentAsync(userId, pageId, contentLevel, cancellationToken);
            return new JsonResult(response);
        }

        [HttpPost]
        [Route("page/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> PublishPageAsync(Guid pageId, CancellationToken cancellationToken)
        {
            var pageGuid = await _contentService.PublishContentAsync(pageId, cancellationToken);
            return Ok(pageGuid);
        }

        [HttpDelete]
        [Route("page/{pageId:guid}/discard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> DiscardPageDraftAsync(Guid pageId, CancellationToken cancellationToken)
        {
            var response = await _contentService.DiscardDraftContentAsync(pageId, cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        [Route("page/{userId:guid}/{pageId:guid}/user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> UpdateUserEditingContentAsync(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            var response = await _contentService.UpdateUserEditingContentAsync(userId, pageId, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route("page/{userId:guid}/{pageId:guid}/editStatus")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> CheckPageEditStatusAsync(Guid userId, Guid pageId, CancellationToken cancellationToken)
        {
            var response = await _contentService.CheckPageEditStatusAsync(userId, pageId, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route("block")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ContentModelData>>))]
        public async Task<IActionResult> GetAllBlocksAsync()
        {
            var blocks = await _contentDataProvider.GetAllBlocksAsync();
            return new JsonResult(blocks);
        }

        [HttpPost]
        [Route("block/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> CreateBlockAsync(Guid userId, [FromBody] BlockCreateRequest createRequest, CancellationToken cancellationToken)
        {
            var blockGuid = await _contentService.CreateBlockAsync(userId, createRequest, cancellationToken);
            return new JsonResult(blockGuid); ;
        }

        [HttpDelete]
        [Route("block/{userId:guid}/{blockId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> DeleteBlockAsync(Guid userId, Guid blockId, CancellationToken cancellationToken, int? contentLevel = null)
        {
            var response = await _contentService.DeleteContentAsync(userId, blockId, contentLevel, cancellationToken);
            return new JsonResult(response);
        }

        [HttpGet]
        [Route("block/{blockId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModelData>))]
        public async Task<IActionResult> GetBlockAsync(Guid blockId)
        {
            var block = await _contentDataProvider.GetBlockAsync(blockId);
            return new JsonResult(block);
        }

        [HttpGet]
        [Route("block/{blockId:guid}/content")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetAllBlockFieldsAsync(Guid blockId)
        {
            var block = await _contentDataProvider.GetBlockContentAsync(blockId);
            return new JsonResult(block);
        }

        [HttpGet]
        [Route("block/{blockId:guid}/labels")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetBlockLabelsAsync(Guid blockId)
        {
            var block = await _contentDataProvider.GetBlockLabelsAsync(blockId);
            return new JsonResult(block);
        }

        [HttpGet]
        [Route("block/{blockId:guid}/content/placeholder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetBlockFieldsPlaceholderValuesAsync(Guid blockId)
        {
            var block = await _contentDataProvider.GetBlockContentPlaceholderValuesAsync(blockId);
            return new JsonResult(block);
        }

        [HttpGet]
        [Route("block/{blockId:guid}/labels/placeholder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetBlockLabelsPlaceholderValuesAsync(Guid blockId)
        {
            var block = await _contentDataProvider.GetBlockLabelsPlaceholderValuesAsync(blockId);
            return new JsonResult(block);
        }

        [HttpGet]
        [Route("template/{templateId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ContentModelData>))]
        public async Task<IActionResult> GetTemplateAsync(Guid templateId)
        {
            var template = await _contentDataProvider.GetTemplateAsync(templateId);
            return new JsonResult(template);
        }

        [HttpGet]
        [Route("template")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<ContentModelData>>))]
        public async Task<IActionResult> GetTemplatesAsync()
        {
            var templates = await _contentDataProvider.GetTemplatesAsync();
            return new JsonResult(templates);
        }

        [HttpGet]
        [Route("sitemap/{pageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SitemapGroupItemModelData>))]
        public async Task<IActionResult> GetSiteMapAsync(Guid pageId)
        {
            var sitemap = await _contentDataProvider.GetSiteMapAsync(pageId);
            return new JsonResult(sitemap);
        }
    }
}
