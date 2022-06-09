using Umbraco9ContentApi.Core.Models.Content;


namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsValidationService
    {
        /// <summary>
        /// Validates the content model data.
        /// </summary>
        /// <param name="contentModel">The content model.</param>
        void ValidateContentModelData(ContentModelData contentModel);
    }
}
