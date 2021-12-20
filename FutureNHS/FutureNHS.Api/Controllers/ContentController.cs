using Microsoft.AspNetCore.Mvc;
using FutureNHS.Infrastructure.Repositories.Read.Interfaces;

namespace FutureNHS.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [ApiVersion("1.0")]
    public sealed class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private readonly IImageDataProvider _imageDataProvider;

        public ContentController(ILogger<ContentController> logger, IImageDataProvider imageDataProvider)
        {
            _logger = logger;
            _imageDataProvider = imageDataProvider;
        }

        [HttpGet]
        [Route("image/{id}")]
        public async Task<IActionResult> GetImageAsync(Guid id)
        {
            var image = await _imageDataProvider.GetImageAsync(id);

            return File(image.Data, image.MediaType);
        }

    }
}