using FutureNHS.Api.Attributes;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.Models.Folder;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Ganss.XSS;
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
        private readonly IEtagService _etagService;
        private readonly IHtmlSanitizer _htmlSanitizer;


        public FolderController(ILogger<FolderController> logger, IFileAndFolderDataProvider fileAndFolderDataProvider, IFolderService folderService,
            IPermissionsService permissionsService, IEtagService etagService, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileAndFolderDataProvider = fileAndFolderDataProvider ?? throw new ArgumentNullException(nameof(fileAndFolderDataProvider));
            _folderService = folderService ?? throw new ArgumentNullException(nameof(folderService)); ;
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _etagService = etagService ?? throw new ArgumentNullException(nameof(etagService));
            _htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer));
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
        public async Task<IActionResult> CreateFolderAsync(Guid userId, string slug, Folder folder, CancellationToken cancellationToken)
        {
            var sanitisedFolder = new Folder
            {
                Name = _htmlSanitizer.Sanitize(folder.Name),
                Description = _htmlSanitizer.Sanitize(folder.Description)
            };

            var folderId = await _folderService.CreateFolderAsync(userId, slug, sanitisedFolder, cancellationToken);

            return Ok(folderId);
        }

        [HttpPost]
        [Route("users/{userId:guid}/groups/{slug}/folders/{folderId:guid}")]
        public async Task<IActionResult> CreateFolderAsync(Guid userId, string slug, Guid folderId, Folder folder, CancellationToken cancellationToken)
        {            
            var sanitisedFolder = new Folder
            {
                Name = _htmlSanitizer.Sanitize(folder.Name),
                Description = _htmlSanitizer.Sanitize(folder.Description)
            };

			var childFolderId = await _folderService.CreateChildFolderAsync(userId, slug, folderId, sanitisedFolder, cancellationToken);

            return Ok(childFolderId);
        }

        [HttpGet]
        [Route("users/{userId:guid}/groups/{slug}/folders/{folderId:guid}/update")]
        [TypeFilter(typeof(ETagFilter))]
        public async Task<IActionResult> GetUpdateFolderAsync(Guid userId, string slug, Guid folderId, CancellationToken cancellationToken)
        {
            var folder = await _folderService.GetFolderAsync(userId, slug, folderId, cancellationToken);

            if (folder is null)
            {
                return NotFound();
            }
            
            return Ok(folder);
        }

        [HttpPut]
        [Route("users/{userId:guid}/groups/{slug}/folders/{folderId:guid}/update")]
        public async Task<IActionResult> UpdateFolderAsync(Guid userId, string slug, Guid folderId, Folder folder, CancellationToken cancellationToken)
        {
            var sanitisedFolder = new Folder
            {
                Name = _htmlSanitizer.Sanitize(folder.Name),
                Description = _htmlSanitizer.Sanitize(folder.Description)
            };
            var rowVersion = _etagService.GetIfMatch();

            await _folderService.UpdateFolderAsync(userId, slug, folderId, sanitisedFolder, rowVersion, cancellationToken);

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