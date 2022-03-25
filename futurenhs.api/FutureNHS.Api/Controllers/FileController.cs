using System.Net;
using FutureNHS.Api.Attributes;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.Extensions;
using FutureNHS.Api.Helpers;
using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Helpers;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class FileController : ControllerBase
    {
        private readonly string _fqdn;
        private readonly ILogger<FileController> _logger;
        private readonly IFileAndFolderDataProvider _fileAndFolderDataProvider;
        private readonly IFileService _fileService;
        private readonly IPermissionsService _permissionsService;
        private readonly IFileServerService _fileServerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileController(IHttpContextAccessor httpContextAccessor, ILogger<FileController> logger, IFileAndFolderDataProvider fileAndFolderDataProvider, IPermissionsService permissionsService, IFileService fileService, IFileServerService fileServerService, IOptionsSnapshot<ApplicationGateway> gatewayConfig)
        {
            _logger = logger;
            _fileAndFolderDataProvider = fileAndFolderDataProvider;
            _fileService = fileService;
            _permissionsService = permissionsService;
            _fileServerService = fileServerService;
            _httpContextAccessor = httpContextAccessor;
            _fqdn = gatewayConfig.Value.FQDN;
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
        [Route("users/{userId}/groups/{slug}/files/{id:guid}/download")]

        public async Task<IActionResult> GetFileDownloadUrlAsync(Guid userId,string slug, Guid id, CancellationToken cancellationToken)
        {
            var file = await _fileService.GetFileDownloadUrl(userId, slug,id, cancellationToken);

            if (file is null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        [HttpGet]
        [Route("users/{userId}/groups/{slug}/files/{id:guid}/view")]

        public async Task<IActionResult> GetViewCollaboraUrlAsync(Guid userId, string slug, Guid id, CancellationToken cancellationToken)
        {
            var file = await _fileServerService.GetCollaboraFileUrl(userId,slug, "view", id, HttpContext.Request, cancellationToken);

            return Ok(file);
        }

        //[HttpPost]
        //[Route("users/{userId:guid}/groups/{slug}/folders/{folderId:guid}/files")]

        //public async Task<IActionResult> CreateFileAsync(Guid userId, string slug, Guid folderId, FutureNHS.Api.Models.File.File file, CancellationToken cancellationToken)
        //{
        //    await _fileService.CreateFileAsync(userId, slug, folderId, file, cancellationToken);

        //    return Ok(file);
        //}

        [HttpOptions]
        [Route("users/{userId:guid}/groups/{slug}/folders/{folderId:guid}/files")]
        public async Task<IActionResult> PreFlightRoute()
        {
            return NoContent();
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        [Route("users/{userId:guid}/groups/{slug}/folders/{folderId:guid}/files")]
        public async Task<IActionResult> UploadDocumentStream(Guid userId, string slug, Guid folderId, CancellationToken cancellationToken)
        {
            if (Request.ContentType != null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest("The data submitted is not in the multiform format");
            }

            await _fileService.UploadFileMultipartDocument(userId,slug,folderId,HttpContext.Request.Body, HttpContext.Request.ContentType, cancellationToken);
            
            return Ok();
        }
    }
}