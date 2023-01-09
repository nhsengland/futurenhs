using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record FileVersion
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public UserNavProperty ModifiedByUser { get; init; }
        public DateTime? ModifiedAtUtc { get; init; }
        
    }
}
