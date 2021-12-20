namespace FutureNHS.Infrastructure.Models
{
    public record ImageData
    {
        public Guid Id { get; init; }

        public string? Source => $"/Collaboration/api/v1/image/{Id}";

        public int Height { get; init; }

        public int Width { get; init; }

        public string MediaType { get; init; }
    }
}
