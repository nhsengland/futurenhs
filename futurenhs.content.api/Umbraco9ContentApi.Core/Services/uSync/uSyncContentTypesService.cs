using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco9ContentApi.Core.Constants.uSync;
using Umbraco9ContentApi.Core.Services.uSync;
using Umbraco9ContentApi.Core.Services.uSync.Interface;
using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers.Handlers;
using uSync.Core;

namespace Umbraco9ContentApi.Core.Services
{
    /// <summary>
    /// uSyncContentTypeService.
    /// </summary>
    /// <seealso cref="uSyncBaseService" />
    /// <seealso cref="IuSyncContentTypeService" />
    public class uSyncContentTypesService : uSyncBaseService, IuSyncContentTypeService
    {
        public ContentTypeHandler contentTypeHandler;
        public ILogger<ContentTypeHandler> _logger;
        public IContentTypeService _contentTypeService;
        public AppCaches _appCaches;
        public IEntityService _entityService;
        public IShortStringHelper _shortStringHelper;
        public SyncFileService _uSyncFileService;
        public uSyncEventService _uSyncEventService;
        public uSyncConfigService _uSyncConfigService;
        public ISyncItemFactory _syncItemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="uSyncContentTypesService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="contentTypeService">The content type service.</param>
        /// <param name="appCaches">The application caches.</param>
        /// <param name="entityService">The entity service.</param>
        /// <param name="shortStringHelper">The short string helper.</param>
        /// <param name="uSyncFileService">The u synchronize file service.</param>
        /// <param name="uSyncEventService">The u synchronize event service.</param>
        /// <param name="uSyncConfigService">The u synchronize configuration service.</param>
        /// <param name="syncItemFactory">The synchronize item factory.</param>
        public uSyncContentTypesService(ILogger<ContentTypeHandler> logger, IContentTypeService contentTypeService, AppCaches appCaches, IEntityService entityService, IShortStringHelper shortStringHelper, SyncFileService uSyncFileService, uSyncEventService uSyncEventService, uSyncConfigService uSyncConfigService, ISyncItemFactory syncItemFactory)
        {

            _logger = logger;
            _contentTypeService = contentTypeService;
            _appCaches = appCaches;
            _entityService = entityService;
            _shortStringHelper = shortStringHelper;
            _uSyncFileService = uSyncFileService;
            _uSyncEventService = uSyncEventService;
            _uSyncConfigService = uSyncConfigService;
            _syncItemFactory = syncItemFactory;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<uSyncAction>> ImportAsync()
        {
            contentTypeHandler = new ContentTypeHandler(
                _logger,
                _entityService,
                _contentTypeService,
                _appCaches,
                _shortStringHelper,
                _uSyncFileService,
                _uSyncEventService,
                _uSyncConfigService,
                _syncItemFactory);

            var importResults = contentTypeHandler.ImportAll(uSyncDirectoryPaths.ContentTypes, new HandlerSettings(), default);

            return importResults;
        }
    }
}