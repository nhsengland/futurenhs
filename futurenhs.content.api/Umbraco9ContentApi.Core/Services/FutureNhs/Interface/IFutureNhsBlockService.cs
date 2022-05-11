using Umbraco9ContentApi.Core.Models.Requests;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsBlockService
    {
        Task<IEnumerable<string?>> GetBlockPlaceholderValuesAsync(Guid blockId, string propertyGroupAlias, CancellationToken cancellationToken);
        Task<Umbraco.Cms.Core.Models.IContent> CreateBlockAsync(CreateBlockRequest createRequest, Guid blocksDataSourceFolderGuid, CancellationToken cancellationToken);
    }
}
