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
    /// uSyncDataTypeService.
    /// </summary>
    /// <seealso cref="uSyncBaseService" />
    /// <seealso cref="IuSyncDataTypeService" />
    public sealed class uSyncDataTypeService : uSyncBaseService, IuSyncDataTypeService
    {
        public DataTypeHandler dataTypeHandler;
        public ILogger<DataTypeHandler> _logger;
        public IDataTypeService _dataTypeService;
        public AppCaches _appCaches;
        public IEntityService _entityService;
        public IShortStringHelper _shortStringHelper;
        public SyncFileService _uSyncFileService;
        public uSyncEventService _uSyncEventService;
        public uSyncConfigService _uSyncConfigService;
        public ISyncItemFactory _syncItemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="uSyncDataTypeService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dataTypeService">The data type service.</param>
        /// <param name="appCaches">The application caches.</param>
        /// <param name="entityService">The entity service.</param>
        /// <param name="shortStringHelper">The short string helper.</param>
        /// <param name="uSyncFileService">The u synchronize file service.</param>
        /// <param name="uSyncEventService">The u synchronize event service.</param>
        /// <param name="uSyncConfigService">The u synchronize configuration service.</param>
        /// <param name="syncItemFactory">The synchronize item factory.</param>
        public uSyncDataTypeService(ILogger<DataTypeHandler> logger, IDataTypeService dataTypeService, AppCaches appCaches, IEntityService entityService, IShortStringHelper shortStringHelper, SyncFileService uSyncFileService, uSyncEventService uSyncEventService, uSyncConfigService uSyncConfigService, ISyncItemFactory syncItemFactory)
        {
            _logger = logger;
            _dataTypeService = dataTypeService;
            _appCaches = appCaches;
            _entityService = entityService;
            _shortStringHelper = shortStringHelper;
            _uSyncFileService = uSyncFileService;
            _uSyncEventService = uSyncEventService;
            _uSyncConfigService = uSyncConfigService;
            _syncItemFactory = syncItemFactory;
        }

        /// <inheritdoc />
        public  IEnumerable<uSyncAction> Import()
        {
            dataTypeHandler = new DataTypeHandler(
                _logger,
                _entityService,
                _dataTypeService,
                _appCaches,
                _shortStringHelper,
                _uSyncFileService,
                _uSyncEventService,
                _uSyncConfigService,
                _syncItemFactory);

            var importResults = dataTypeHandler.ImportAll(uSyncDirectoryPaths.DataTypes, new HandlerSettings(), default);

            return importResults;
        }
    }
}
