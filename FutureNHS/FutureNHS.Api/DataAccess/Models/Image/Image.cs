namespace FutureNHS.Api.DataAccess.Models
{
    public record Image : ImageData
    {
        public byte[] Data { get; init; }

    }
}
