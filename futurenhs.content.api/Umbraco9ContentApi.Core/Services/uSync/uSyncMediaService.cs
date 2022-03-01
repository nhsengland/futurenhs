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
    /// uSyncMediaService.
    /// </summary>
    /// <seealso cref="uSyncBaseService" />
    /// <seealso cref="IuSyncMediaService" />
    public class uSyncMediaService : uSyncBaseService, IuSyncMediaService
    {
        public MediaHandler mediaHandler;
        public ILogger<MediaHandler> _logger;
        public IMediaService _mediaService;
        public AppCaches _appCaches;
        public IEntityService _entityService;
        public IShortStringHelper _shortStringHelper;
        public SyncFileService _uSyncFileService;
        public uSyncEventService _uSyncEventService;
        public uSyncConfigService _uSyncConfigService;
        public ISyncItemFactory _syncItemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="uSyncMediaService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mediaService">The media service.</param>
        /// <param name="appCaches">The application caches.</param>
        /// <param name="entityService">The entity service.</param>
        /// <param name="shortStringHelper">The short string helper.</param>
        /// <param name="uSyncFileService">The u synchronize file service.</param>
        /// <param name="uSyncEventService">The u synchronize event service.</param>
        /// <param name="uSyncConfigService">The u synchronize configuration service.</param>
        /// <param name="syncItemFactory">The synchronize item factory.</param>
        public uSyncMediaService(ILogger<MediaHandler> logger, IMediaService mediaService, AppCaches appCaches, IEntityService entityService, IShortStringHelper shortStringHelper, SyncFileService uSyncFileService, uSyncEventService uSyncEventService, uSyncConfigService uSyncConfigService, ISyncItemFactory syncItemFactory)
        {

            _logger = logger;
            _mediaService = mediaService;
            _appCaches = appCaches;
            _entityService = entityService;
            _shortStringHelper = shortStringHelper;
            _uSyncFileService = uSyncFileService;
            _uSyncEventService = uSyncEventService;
            _uSyncConfigService = uSyncConfigService;
            _syncItemFactory = syncItemFactory;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<uSyncAction>> Import()
        {
            mediaHandler = new MediaHandler(
                _logger,
                _entityService,
                _mediaService,
                _appCaches,
                _shortStringHelper,
                _uSyncFileService,
                _uSyncEventService,
                _uSyncConfigService,
                _syncItemFactory);

            var importResults = mediaHandler.ImportAll(uSyncDirectoryPaths.Media, new HandlerSettings(), default);

            return importResults;
        }
    }
}
