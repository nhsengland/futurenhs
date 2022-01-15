using FutureNHS.Api.DataAccess.Models.GroupUser;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class FileFolderController : ControllerBase
    {
        private readonly ILogger<FileFolderController> _logger;
        private readonly IFileAndFolderDataProvider _fileAndFolderDataProvider;
        private readonly IPermissionsService _permissionsService;

        public FileFolderController(ILogger<FileFolderController> logger, IFileAndFolderDataProvider fileAndFolderDataProvider, IPermissionsService permissionsService)
        {
            _logger = logger;
            _fileAndFolderDataProvider = fileAndFolderDataProvider;
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
        [Route("users/{userId}/groups/{slug}/files/{id:guid}")]

        public async Task<IActionResult> GetFileAsync(Guid id, CancellationToken cancellationToken)
        {
            var file = await _fileAndFolderDataProvider.GetFileAsync(id, cancellationToken);

            if (file is null)
            { 
                return NotFound();
            }

            return Ok(file);
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