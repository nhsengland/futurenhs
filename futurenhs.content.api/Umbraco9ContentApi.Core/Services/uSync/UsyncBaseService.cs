using uSync.BackOffice;

namespace Umbraco9ContentApi.Core.Services.uSync
{
    public class uSyncBaseService
    {
        protected IEnumerable<uSyncAction> uSyncChanges = new List<uSyncAction>();
        protected IEnumerable<uSyncAction> uSyncSuccessful = new List<uSyncAction>();
        protected IEnumerable<uSyncAction> uSyncFailed = new List<uSyncAction>();
    }
}