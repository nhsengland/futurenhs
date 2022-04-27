namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsBlockService
    {
        Task<IEnumerable<string>> GetBlockPlaceholderValuesAsync(Guid blockId);
    }
}
