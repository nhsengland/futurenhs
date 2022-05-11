using Umbraco9ContentApi.Core.Models.Content;


namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsValidationService
    {
        void ValidateContentModel(ContentModel contentModel);
    }
}
