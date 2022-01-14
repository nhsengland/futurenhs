using FutureNHS.Api.DataAccess.Models.GroupUser;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        [Route("folders/{id:guid}")]

        public async Task<IActionResult> GetFolderAsync(Guid id, CancellationToken cancellationToken)
        {
           var folder = await _fileAndFolderDataProvider.GetFolderAsync(id, cancellationToken);

            return Ok(folder);
        }

        [HttpGet]
        [Route("files/{id:guid}")]

        public async Task<IActionResult> GetFileAsync(Guid id, CancellationToken cancellationToken)
        {
            var folder = await _fileAndFolderDataProvider.GetFileAsync(id, cancellationToken);

            return Ok(folder);
        }

        [HttpGet]
        [Route("folders/{id:guid}/contents")]

        public async Task<IActionResult> GetFolderContentsAsync(Guid id, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
        {
            var route = Request.Path.Value;

            var (total, folderContents) = await _fileAndFolderDataProvider.GetFolderContentsAsync(id, filter.Offset, filter.Limit, cancellationToken);
            
            var pagedResponse = PaginationHelper.CreatePagedResponse(folderContents, filter, total, route);

            return Ok(pagedResponse);
        }
    }
}