using FutureNHS.Api.DataAccess.Models.Content;

namespace FutureNHS.Api.DataAccess.Models.Requests
{
    public class GeneralWebPageUpdateRequest
    {
        public ContentModelData[]? Blocks { get; set; }
    }
}