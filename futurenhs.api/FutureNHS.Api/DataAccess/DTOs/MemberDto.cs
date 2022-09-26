using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public record MemberDto : BaseData
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string? Surname { get; set; }
        public string Pronouns { get; set; }       
        public string Email { get; set; }
        public Guid? ImageId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAtUTC { get; set; }
        public bool AgreedToTerms { get; set; }
    }
}
