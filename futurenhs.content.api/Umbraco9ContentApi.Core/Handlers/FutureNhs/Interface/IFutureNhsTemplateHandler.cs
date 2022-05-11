namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using Umbraco9ContentApi.Core.Models.Content;
    using Umbraco9ContentApi.Core.Models.Response;

    public interface IFutureNhsTemplateHandler
    {
        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<ContentModel> GetTemplate(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ApiResponse<IEnumerable<ContentModel>> GetAllTemplates(CancellationToken cancellationToken);
    }
}
