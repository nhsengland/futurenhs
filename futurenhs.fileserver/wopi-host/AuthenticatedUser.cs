using System;
using System.Text.Json.Serialization;

namespace FutureNHS.WOPIHost
{
    public sealed record AuthenticatedUser(Guid Id, string? FullName)
    {
        public string? EmailAddress { get; init; }
        public UserAvatarMetadata? UserAvatar { get; init; }
        public UserFileMetadata? FileMetadata { get; init; }

        //[JsonIgnore] public UserFileMetadata? FileMetadata { get; init; }
        //public static AuthenticatedUser Empty { get; } = new();

        //[JsonIgnore] public bool IsEmpty => ReferenceEquals(this, Empty) || Id is null || Guid.Empty == Id;

        //[JsonConstructor]
        //public AuthenticatedUser() { }

        //[JsonInclude] public Guid? Id { get; init; }
        //[JsonInclude] public string? FullName { get; init; }
        //[JsonInclude] public string? EmailAddress { get; init; }
        //[JsonInclude] public UserAvatarMetadata? UserAvatar { get; init; }

        //[JsonIgnore] public UserFileMetadata? FileMetadata { get; init; }
    }

    public sealed record UserAvatarMetadata(Uri Source)
    {
//        [JsonInclude] public Uri? Source { get; init; }
    }
}
