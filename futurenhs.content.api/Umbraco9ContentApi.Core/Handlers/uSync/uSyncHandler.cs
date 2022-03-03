namespace Umbraco9ContentApi.Core.Handlers.uSync
{
    using global::uSync.BackOffice;
    using Umbraco9ContentApi.Core.Handlers.uSync.Interface;
    using Umbraco9ContentApi.Core.Services.uSync.Interface;

    /// <summary>
    /// Handler for uSync services.
    /// </summary>
    /// <seealso cref="IuSyncHandler" />
    public sealed class uSyncHandler : IuSyncHandler
    {
        private IEnumerable<uSyncAction> uSyncImportResults = new List<uSyncAction>();
        private int contentTypeCount = 0;
        private int dataTypeCount = 0;
        private int MediaTypeCount = 0;
        private int contentCount = 0;
        private int mediaCount = 0;

        private readonly IuSyncContentService _uSyncContentService;
        private readonly IuSyncContentTypeService _uSyncContentTypeService;
        private readonly IuSyncDataTypeService _uSyncDataTypeService;
        private readonly IuSyncMediaTypeService _uSyncMediaTypeService;
        private readonly IuSyncMediaService _uSyncMediaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="uSyncHandler"/> class.
        /// </summary>
        /// <param name="uSyncContentService">The u synchronize content service.</param>
        /// <param name="uSyncContentTypeService">The u synchronize content type service.</param>
        /// <param name="uSyncDataTypeService">The u synchronize data type service.</param>
        /// <param name="uSyncMediaTypeService">The u synchronize media type service.</param>
        /// <param name="uSyncMediaService">The u synchronize media service.</param>
        public uSyncHandler(IuSyncContentService uSyncContentService, IuSyncContentTypeService uSyncContentTypeService, IuSyncDataTypeService uSyncDataTypeService, IuSyncMediaTypeService uSyncMediaTypeService, IuSyncMediaService uSyncMediaService)
        {
            _uSyncContentService = uSyncContentService;
            _uSyncContentTypeService = uSyncContentTypeService;
            _uSyncDataTypeService = uSyncDataTypeService;
            _uSyncMediaTypeService = uSyncMediaTypeService;
            _uSyncMediaService = uSyncMediaService;
        }

        /// <inheritdoc />
        public async Task<bool> RunImportAsync()
        {
            try
            {
                uSyncImportResults = await _uSyncContentTypeService.ImportAsync();
                if (uSyncImportResults is not null) contentTypeCount = uSyncImportResults.Where(x => x.Success).Count();

                uSyncImportResults = await _uSyncDataTypeService.ImportAsync();
                if (uSyncImportResults is not null) dataTypeCount = uSyncImportResults.Where(x => x.Success).Count();

                uSyncImportResults = await _uSyncMediaTypeService.ImportAsync();
                if (uSyncImportResults is not null) MediaTypeCount = uSyncImportResults.Where(x => x.Success).Count();

                uSyncImportResults = await _uSyncContentService.ImportAsync();
                if (uSyncImportResults is not null) contentCount = uSyncImportResults.Where(x => x.Success).Count();

                uSyncImportResults = await _uSyncMediaService.ImportAsync();
                if (uSyncImportResults is not null) mediaCount = uSyncImportResults.Where(x => x.Success).Count();
            }
            catch (Exception e)
            {
                // handle
                return false;
            }

            //:TODO map content ids.
            return true;
        }

    }
}

