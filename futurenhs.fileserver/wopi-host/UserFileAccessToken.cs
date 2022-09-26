using System;
using System.Text.Json.Serialization;

namespace FutureNHS.WOPIHost
{
    public enum FileAccessPermission
    {
        View = 0,
        Edit = 1
    }

    public sealed record UserFileAccessToken(Guid Id, AuthenticatedUser UserScope, FileAccessPermission FileAccessPermission, DateTimeOffset ExpiresAtUtc)
    {
        //public static UserFileAccessToken Empty { get; } = new();

        //private UserFileAccessToken() { }

        //[JsonConstructor]

        //public UserFileAccessToken(Guid id, AuthenticatedUser userScope, DateTimeOffset expiresAtUtc)
        //{
        //    if (Guid.Empty == id) throw new ArgumentNullException("id");
        //    if (userScope is null || userScope.IsEmpty) throw new ArgumentNullException(nameof(userScope));
        //    if (userScope.FileMetadata is null || userScope.FileMetadata.IsEmpty) throw new ArgumentOutOfRangeException(nameof(userScope));

        //    if (expiresAtUtc == DateTimeOffset.MinValue) throw new ArgumentOutOfRangeException(nameof(expiresAtUtc));
        //    if (expiresAtUtc == DateTimeOffset.MaxValue) throw new ArgumentOutOfRangeException(nameof(expiresAtUtc));

        //    Id = id;
        //    UserScope = userScope;
        //    ExpiresAtUtc = expiresAtUtc;
        //}

        //[JsonIgnore]
        //public bool IsEmpty => object.ReferenceEquals(this, Empty);

        //public Guid? Id { get; }
        //public DateTimeOffset? ExpiresAtUtc { get; }
        //public AuthenticatedUser? UserScope { get; }
    }
}
