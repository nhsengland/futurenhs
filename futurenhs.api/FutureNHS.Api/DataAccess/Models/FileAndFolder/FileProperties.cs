namespace FutureNHS.Api.DataAccess.Models.FileAndFolder
{
    public record FileProperties
    {
        public string? MediaType { get; init; }

        public string FileExtension  { get; init; }
    }
}
