using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed record DomainDto : BaseData
    {
        public Guid Id { get; set; }
        public string EmailDomain { get; set; }
        public bool IsDeleted { get; set; }
    }
}
