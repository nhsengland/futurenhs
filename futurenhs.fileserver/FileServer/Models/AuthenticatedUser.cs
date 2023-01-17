using FutureNHS.WOPIHost;
using System;
using System.Text.Json.Serialization;
using FileServer.Enums;

namespace FileServer.Models
{
    public sealed record AuthenticatedUser
    {
        public Guid Id { get; init; }
        public string? FullName { get; init; }
        public string? EmailAddress { get; init; }
        
        public string? AvatarUrl { get; init; }
        public Guid? AccessToken{ get; init; }
        
        public FileAccessPermission UserAccess{ get; init; }
        public UserFileMetadata? FileMetadata { get; init; }
    }

}
