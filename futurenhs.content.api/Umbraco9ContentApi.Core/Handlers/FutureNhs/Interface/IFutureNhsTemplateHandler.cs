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
        /// <returns>Specified template.</returns>
        Task<ApiResponse<ContentModel>> GetTemplateAsync(Guid id);

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <returns>All templates.</returns>
        Task<ApiResponse<IEnumerable<ContentModel>>> GetAllTemplatesAsync();
    }
}