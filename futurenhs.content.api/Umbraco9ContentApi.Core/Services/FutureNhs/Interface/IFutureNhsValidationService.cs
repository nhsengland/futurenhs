using Umbraco9ContentApi.Core.Models;
using Umbraco9ContentApi.Core.Models.Content;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsValidationService
    {
        void ValidatePageContentModel(PageModel pageContentModel);
        void ValidateContentModel(ContentModel contentModel);
    }
}
