using uSync.BackOffice;

namespace Umbraco9ContentApi.Core.Services.uSync.Interface
{
    public interface IuSyncBaseService
    {
        /// <summary>
        /// Initiate import.
        /// </summary>
        /// <returns></returns>
        IEnumerable<uSyncAction> Import();
    }
}
