using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.Models.Discussion;
using FutureNHS.Api.Models.Folder;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class FolderController : ControllerBase
    {
        private readonly ILogger<FolderController> _logger;
        private readonly IFileAndFolderDataProvider _fileAndFolderDataProvider;
        private readonly IFolderService _folderService;
        private readonly IPermissionsService _permissionsService;

        public FolderController(ILogger<FolderController> logger, IFileAndFolderDataProvider fileAndFolderDataProvider, IFolderService folderService, IPermissionsService permissionsService)
        {
            _logger = logger;
            _fileAndFolderDataProvider = fileAndFolderDataProvider;
            _folderService = folderService;
            _permissionsService = permissionsService;
        }

        [HttpGet]
        [Route("users/{userId}/groups/{slug}/folders")]

        public async Task<IActionResult> GetFolderContentsAsync(string slug, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;
            var (total, folderContents) = await _fileAndFolderDataProvider.GetRootFoldersAsync(slug, filter.Offset, filter.Limit, cancellationToken);

            var pagedResponse = PaginationHelper.CreatePagedResponse(folderContents, filter, total, route);

            return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/folders")]

        public async Task<IActionResult> CreateFolderAsync(Guid userId, string slug,Folder folder, CancellationToken cancellationToken)
        {

            await _folderService.CreateFolderAsync(userId, slug, folder, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/folders/{id:guid}")]

        public async Task<IActionResult> CreateFolderAsync(Guid userId, string slug, Guid id, Folder folder, CancellationToken cancellationToken)
        {

            await _folderService.CreateChildFolderAsync(userId, slug,id, folder, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("users/{userId}/groups/{slug}/folders/{id:guid}")]

        public async Task<IActionResult> GetFolderAsync(Guid id, CancellationToken cancellationToken)
        {
           var folder = await _fileAndFolderDataProvider.GetFolderAsync(id, cancellationToken);

            if (folder is null)
            {
                return NotFound();
            }

            return Ok(folder);
        }


        [HttpGet]
        [Route("users/{userId}/groups/{slug}/folders/{id:guid}/contents")]

        public async Task<IActionResult> GetFolderContentsAsync(Guid id, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (total, folderContents) = await _fileAndFolderDataProvider.GetFolderContentsAsync(id, filter.Offset, filter.Limit, cancellationToken);
            
            var pagedResponse = PaginationHelper.CreatePagedResponse(folderContents, filter, total, route);

            return Ok(pagedResponse);
        }
    }
}