using FileServer.Enums;

namespace FileServer.Models
{
    public sealed record UserFileAccessToken
    {
        public Guid Id { get; init; }
        public AuthenticatedUser User { get; init; }
        public FileAccessPermission FileAccessPermission { get; init; }
        public DateTimeOffset ExpiresAtUtc { get; init; }
    }
}
