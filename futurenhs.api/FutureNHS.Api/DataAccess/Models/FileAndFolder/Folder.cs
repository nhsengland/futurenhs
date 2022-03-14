namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public sealed record Folder : BaseData
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
        public IEnumerable<FolderPathItem>? Path { get; init; }
        public Shared.Properties? FirstRegistered { get; init; }
        //public Properties LastUpdated  { get; init; }
    }
}
