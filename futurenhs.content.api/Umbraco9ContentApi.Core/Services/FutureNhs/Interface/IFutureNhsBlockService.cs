using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco9ContentApi.Core.Models.Blocks;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsBlockService
    {
        Task<IEnumerable<string>> GetBlockPlaceholderValuesAsync(Guid blockId);
        Task<BlockModel> ResolvePublishedBlockAsync(IPublishedContent block);
    }
}
