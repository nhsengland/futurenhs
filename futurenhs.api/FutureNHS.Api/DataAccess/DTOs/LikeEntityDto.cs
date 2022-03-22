using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record LikeEntityDto : BaseData
    {
        public Guid Id { get; set; }
    }
}
