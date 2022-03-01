namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using ContentModel = UmbracoContentApi.Core.Models.ContentModel;

    public interface IFutureNhsBlockHandler
    {
        /// <summary>
        /// Gets all blocks.
        /// </summary>
        /// <returns>All blocks.</returns>
        Task<IEnumerable<ContentModel>> GetAllBlocks();
    }
}