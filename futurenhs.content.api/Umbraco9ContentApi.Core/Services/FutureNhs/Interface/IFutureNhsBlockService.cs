using Umbraco.Cms.Core.Models;
using Umbraco9ContentApi.Core.Models.Requests;
using ContentModel = Umbraco9ContentApi.Core.Models.Content.ContentModel;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsBlockService
    {
        IEnumerable<string> GetBlockPlaceholderValues(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken);
        IContent CreateBlock(CreateBlockRequest createRequest, CancellationToken cancellationToken);
        IContent UpdateBlock(ContentModel block, CancellationToken cancellationToken);
        IEnumerable<ContentModel> GetChildBlocks(IEnumerable<ContentModel> blocks, CancellationToken cancellationToken);
    }
}
