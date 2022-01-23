namespace FutureNHS.Api.DataAccess.Models
{
    public record ImageData
    {
        public Guid Id { get; init; }

        public string? Source => $@"https://sacdsfnhsdevuksouthpub.blob.core.windows.net/images/{FileName}";

        public int Height { get; init; }

        public int Width { get; init; }

        public string FileName { get; init; }

        public string MediaType { get; init; }
    }
}
