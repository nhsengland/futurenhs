using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco9ContentApi.Core.Models.Dto
{
    public class GeneralWebPageDto
    {
        public string PageName { get; set; }
        public Guid PageParentId { get; set; }
        public string DocumentTypeAlias { get { return GeneralWebPage.ModelTypeAlias; } }
    }
}
