namespace Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface
{
    using UmbracoContentApi.Core.Models;

    public interface IFutureNhsTemplateHandler
    {
        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Specified template.</returns>
        Task<ContentModel> GetTemplate(Guid id);

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <returns>All templates.</returns>
        Task<IEnumerable<ContentModel>> GetAllTemplates();
    }
}