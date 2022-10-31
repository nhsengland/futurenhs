using FutureNHS.WOPIHost;
using System;
using System.Text.Json.Serialization;

namespace FileServer.Models
{
    public sealed record AuthenticatedUser
    {
        public Guid Id { get; init; }
        public string? FullName { get; init; }
        public string? EmailAddress { get; init; }
        public Guid? AccessToken{ get; init; }
        public UserFileMetadata? FileMetadata { get; init; }
    }

}
