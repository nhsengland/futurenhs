namespace Umbraco9ContentApi.Core.Handlers.FutureNhs
{
    using Interface;
    using Microsoft.Extensions.Configuration;
    using Services.FutureNhs.Interface;
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;


    /// <summary>
    /// The handler that handles block methods and calls the content service.
    /// </summary>
    /// <seealso cref="IFutureNhsBlockHandler" />
    public sealed class FutureNhsBlockHandler : IFutureNhsBlockHandler
    {
        private readonly IConfiguration _config;
        private readonly IFutureNhsContentService _futureNhsContentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureNhsContentHandler"/> class.
        /// </summary>
        /// <param name="futureNhsContentService">The future NHS content service.</param>
        public FutureNhsBlockHandler(IFutureNhsContentService futureNhsContentService, IConfiguration config)
        {
            _futureNhsContentService = futureNhsContentService;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ContentModel>> GetAllBlocksAsync()
        {
            var contentModels = new List<ContentModel>();
            var blocksFolderGuid = _config.GetValue<Guid>("AppKeys:Folders:Blocks");
            var publishedBlocks = await _futureNhsContentService.GetPublishedChildrenAsync(blocksFolderGuid);

            foreach (var block in publishedBlocks)
            {
                contentModels.Add(await _futureNhsContentService.ResolveAsync(block));
            }

            return contentModels;
        }
    }
}