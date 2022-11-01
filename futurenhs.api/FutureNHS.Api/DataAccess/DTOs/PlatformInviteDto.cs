using System.Diagnostics.Contracts;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed class PlatformInviteDto
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        
        public Guid CreatedBy { get; set; }
        public DateTime? ExpiresAtUTC { get; set; }
        public bool IsDeleted { get; init; }
        public byte[] RowVersion { get; set; }
    }
}
