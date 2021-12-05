namespace FutureNHS.Infrastructure.Models
{
    public record Image : ImageData
    {
        public byte[] Data { get; init; }

    }
}
