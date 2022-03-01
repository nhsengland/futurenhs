using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco9ContentApi.Core.Constants.uSync;
using Umbraco9ContentApi.Core.Services.uSync.Interface;
using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers.Handlers;
using uSync.Core;

namespace Umbraco9ContentApi.Core.Services.uSync
{
    /// <summary>
    /// uSyncContentService.
    /// </summary>
    /// <seealso cref="uSyncBaseService" />
    /// <seealso cref="IuSyncContentService" />
    public class uSyncContentService : uSyncBaseService, IuSyncContentService
    {
        public ContentHandler contentHandler;
        public ILogger<ContentHandler> _logger;
        public IContentService _contentService;
        public AppCaches _appCaches;
        public IEntityService _entityService;
        public IShortStringHelper _shortStringHelper;
        public SyncFileService _usyncFileService;
        public uSyncEventService _usyncEventService;
        public uSyncConfigService _usyncConfigService;
        public ISyncItemFactory _syncItemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="uSyncContentService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="contentService">The content service.</param>
        /// <param name="appCaches">The application caches.</param>
        /// <param name="entityService">The entity service.</param>
        /// <param name="shortStringHelper">The short string helper.</param>
        /// <param name="uSyncFileService">The u synchronize file service.</param>
        /// <param name="uSyncEventService">The u synchronize event service.</param>
        /// <param name="uSyncConfigService">The u synchronize configuration service.</param>
        /// <param name="syncItemFactory">The synchronize item factory.</param>
        public uSyncContentService(ILogger<ContentHandler> logger, IContentService contentService, AppCaches appCaches, IEntityService entityService, IShortStringHelper shortStringHelper, SyncFileService usyncFileService, uSyncEventService usyncEventService, uSyncConfigService usyncConfigService, ISyncItemFactory syncItemFactory)
        {
            _logger = logger;
            _contentService = contentService;
            _appCaches = appCaches;
            _entityService = entityService;
            _shortStringHelper = shortStringHelper;
            _usyncFileService = usyncFileService;
            _usyncEventService = usyncEventService;
            _usyncConfigService = usyncConfigService;
            _syncItemFactory = syncItemFactory;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<uSyncAction>> Import()
        {
            contentHandler = new ContentHandler(
                _logger,
                _entityService,
                _contentService,
                _appCaches,
                _shortStringHelper,
                _usyncFileService,
                _usyncEventService,
                _usyncConfigService,
                _syncItemFactory);

            var importResults = contentHandler.ImportAll(uSyncDirectoryPaths.Content, new HandlerSettings(), default);

            return importResults;
        }
    }
}
