using Umbraco.Cms.Core.Models;
using Umbraco9ContentApi.Core.Models.Requests;
using ContentModelData = Umbraco9ContentApi.Core.Models.Content.ContentModelData;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsBlockService
    {
        IEnumerable<string> GetBlockPlaceholderValues(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken);
        IContent CreateBlock(CreateBlockRequest createRequest, CancellationToken cancellationToken);
        IContent UpdateBlock(ContentModelData block, CancellationToken cancellationToken);
        IEnumerable<ContentModelData> GetBlocksAllChildBlocks(IEnumerable<ContentModelData> blocks, CancellationToken cancellationToken);
    }
}
