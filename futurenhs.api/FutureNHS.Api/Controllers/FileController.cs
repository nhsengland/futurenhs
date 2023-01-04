using FutureNHS.Api.Attributes;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class FileController : ControllerIdentityBase
    {
        private readonly ILogger<FileController> _logger;
        private readonly IFileAndFolderDataProvider _fileAndFolderDataProvider;
        private readonly IFileService _fileService;
        private readonly IFileServerService _fileServerService;

        public FileController(ILogger<ControllerIdentityBase> _baseLogger,IUserService userService, IHttpContextAccessor httpContextAccessor, ILogger<FileController> logger, IFileAndFolderDataProvider fileAndFolderDataProvider, IPermissionsService permissionsService, IFileService fileService, IFileServerService fileServerService, IOptionsSnapshot<ApplicationGateway> gatewayConfig) : base(_baseLogger, userService)
        {
            _logger = logger;
            _fileAndFolderDataProvider = fileAndFolderDataProvider;
            _fileService = fileService;
            _fileServerService = fileServerService;
        }

        [HttpGet]
        [Route("groups/{slug}/files/{id:guid}")]

        public async Task<IActionResult> GetFileAsync(Guid id, CancellationToken cancellationToken)
        {
            var file = await _fileAndFolderDataProvider.GetFileAsync(id, cancellationToken);

            if (file is null)
            {
                file = await _fileAndFolderDataProvider.GetFileVersionAsync(id, cancellationToken);
                if (file is null)
                    return NotFound();
            }

            return Ok(file);
        }

        [HttpGet]
        [Route("groups/{slug}/files/{id:guid}/download")]

        public async Task<IActionResult> GetFileDownloadUrlAsync(Guid userId, string slug, Guid id, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var file = await _fileService.GetFileDownloadUrl(identity.MembershipUserId, slug, id, cancellationToken);

            if (file is null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        [HttpGet]
        [Route("groups/{slug}/files/{id:guid}/view")]

        public async Task<IActionResult> GetViewCollaboraUrlAsync(Guid userId, string slug, Guid id, CancellationToken cancellationToken)
        {
            var identity = await GetUserIdentityAsync(cancellationToken);
            var authHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if (authHeader is null)
                throw new FieldAccessException("Authorization header was not found");

            var file = await _fileServerService.GetCollaboraFileUrl(identity.MembershipUserId, slug, "view",  id, authHeader, cancellationToken);

            return Ok(file);
        }

        [HttpGet]
        [Route("files/{id:guid}/auth")]
        public async Task<IActionResult> CheckUserAccessForFile(Guid id, [FromQuery] string permission, CancellationToken cancellationToken = default(CancellationToken))
        {
            var identity = await GetUserIdentityAsync(cancellationToken);

            var userAccess = await _fileService.CheckUserAccess(identity.MembershipUserId, id, permission, cancellationToken);
            return Ok(userAccess);
        }

        [HttpOptions]
        [Route("groups/{slug}/folders/{folderId:guid}/files")]
        public async Task<IActionResult> PreFlightRoute()
        {
            return NoContent();
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        [Route("groups/{slug}/folders/{folderId:guid}/files")]
        public async Task<IActionResult> UploadDocumentStream(string slug, Guid folderId, CancellationToken cancellationToken)
        {
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }

            var identity = await GetUserIdentityAsync(cancellationToken);
            await _fileService.UploadFileMultipartDocument(identity.MembershipUserId, slug, folderId, HttpContext.Request.Body, HttpContext.Request.ContentType, cancellationToken);

            return Ok();
        }
    }
}