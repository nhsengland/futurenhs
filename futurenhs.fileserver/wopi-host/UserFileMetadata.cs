using System;
using System.Text.Json.Serialization;

namespace FutureNHS.WOPIHost
{
    public sealed record UserFileMetadata
    {
        //public static UserFileMetadata Empty { get; } = new();

        //private UserFileMetadata() { }

        //[JsonConstructor]
        //public UserFileMetadata(
        //    Guid fileId,
        //    string title, 
        //    string description,
        //    string groupName, 
        //    string? version, 
        //    string owner,
        //    string name,
        //    string extension, 
        //    ulong sizeInBytes,
        //    string blobName, 
        //    DateTimeOffset lastWriteTimeUtc,
        //    string contentHash, 
        //    FileStatus fileStatus, 
        //    bool userHasViewPermission,
        //    bool userHasEditPermission
        //    )
        //{
        //    if (Guid.Empty == fileId) throw new ArgumentNullException(nameof(fileId));

        //    if (string.IsNullOrWhiteSpace(title))       throw new ArgumentNullException(nameof(title));
        //    if (string.IsNullOrWhiteSpace(description)) throw new ArgumentNullException(nameof(description));
        //    if (string.IsNullOrWhiteSpace(groupName))   throw new ArgumentNullException(nameof(groupName));
        //    if (string.IsNullOrWhiteSpace(owner))       throw new ArgumentNullException(nameof(owner));
        //    if (string.IsNullOrWhiteSpace(name))        throw new ArgumentNullException(nameof(name));
        //    if (string.IsNullOrWhiteSpace(extension))   throw new ArgumentNullException(nameof(extension));
        //    if (string.IsNullOrWhiteSpace(blobName))    throw new ArgumentNullException(nameof(blobName));

        //    if (2 > extension.Length)                   throw new ArgumentOutOfRangeException(nameof(extension), "The file extension needs to be at least 2 characters long (including the period character)");
        //    if (!extension.StartsWith('.'))       throw new ArgumentOutOfRangeException(nameof(extension), "The file extension needs to start with a period character");
        //    if (0 >= sizeInBytes)                       throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "The file size needs to be greater than 0 bytes");

        //    if (lastWriteTimeUtc == DateTimeOffset.MinValue) throw new ArgumentOutOfRangeException(nameof(lastWriteTimeUtc));
        //    if (lastWriteTimeUtc == DateTimeOffset.MaxValue) throw new ArgumentOutOfRangeException(nameof(lastWriteTimeUtc));

        //    FileId = fileId;
        //    Title = title;
        //    Description = description;
        //    Version = version;
        //    Owner = owner;
        //    Name = name;
        //    Extension = extension;
        //    BlobName = blobName;
        //    SizeInBytes = sizeInBytes;
        //    LastWriteTimeUtc = lastWriteTimeUtc;
        //    ContentHash = contentHash;
        //    FileStatus = fileStatus;
        //    GroupName = groupName;
        //    UserHasViewPermission = userHasViewPermission;
        //    UserHasEditPermission = userHasEditPermission && userHasViewPermission;
        //}

        //[JsonIgnore] public bool IsEmpty => ReferenceEquals(this, Empty);

        public Guid FileId { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }

        public string? GroupName { get; init; }

        public string? Name { get; init; }
        public string? FileVersion { get; init; }
        public string? Extension { get; init; }

        public string? BlobName { get; init; }
        public byte[]? ContentHash { get; init; }

        public string? OwnerUserName { get; init; }

        public ulong? SizeInBytes { get; init; }
        public DateTimeOffset? LastWriteTimeUtc { get; init; }

        public bool UserHasViewPermission { get; init; }
        public bool UserHasEditPermission { get; init;}

        public File AsFile() => File.FromId(FileId.ToString(), FileVersion);
    }
}
