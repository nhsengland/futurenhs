namespace Umbraco9ContentApi.Core.Handlers.uSync.Interface
{
    public interface IuSyncHandler
    {
        /// <summary>
        /// Runs the import.
        /// </summary>
        /// <returns></returns>
        Task<bool> RunImport();
    }
}
