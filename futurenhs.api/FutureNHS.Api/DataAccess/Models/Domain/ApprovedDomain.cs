namespace FutureNHS.Api.DataAccess.Models.Domain
{
    public record ApprovedDomain
    {
        public Guid Id { get; init; }
        
        public string EmailDomain { get; init; }
    }
}