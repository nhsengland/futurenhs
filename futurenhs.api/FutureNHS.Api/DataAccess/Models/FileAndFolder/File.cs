using FutureNHS.Api.DataAccess.Models.User;

namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record File
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public IEnumerable<FolderPathItem>? Path { get; init; }
        public Shared.Properties FirstRegistered { get; init; }
        public Shared.Properties LastUpdated  { get; init; }
        public IEnumerable<FileVersion>? Versions { get; init; }
    }
}
