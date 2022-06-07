using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public record PageDto : BaseData
    {
        public string PageName { get; init; }
        public Guid? PageParentId { get; init; }
    }
}
