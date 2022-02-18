using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record FileVersion
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public Shared.Properties FirstRegistered { get; init; }
        public FileProperties AdditionalMetadata { get; init; }
    }
}
