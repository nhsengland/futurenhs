using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public record ContentDto : BaseData
    {
        public string PageName { get; init; }
        public Guid? PageParentId { get; init; }
    }
}
