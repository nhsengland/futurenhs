using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record FileVersion
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public long? Size { get; init; }
        public Shared.Properties LastUpdated  { get; init; }
        
    }
}
