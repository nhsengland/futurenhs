using System;
using System.Text.Json.Serialization;

namespace FutureNHS.WOPIHost
{
    public sealed class FileContentMetadata
    {
        public static FileContentMetadata Empty = new FileContentMetadata();

        private FileContentMetadata() { }

        public FileContentMetadata(string contentVersion, string contentType, byte[] contentHash, ulong contentLength, string? contentEncoding, string? contentLanguage, DateTimeOffset? lastAccessed, DateTimeOffset lastModified, UserFileMetadata fileMetadata)
        {
            if (string.IsNullOrWhiteSpace(contentVersion)) throw new ArgumentNullException(nameof(contentVersion));
            if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentNullException(nameof(contentType));
        
            if (0 > contentLength) throw new ArgumentOutOfRangeException(nameof(contentLength), "Must be greater than zero");

            ContentVersion = contentVersion;
            ContentLength = contentLength;
            ContentType = contentType;
            ContentEncoding = contentEncoding;
            ContentLanguage = contentLanguage;
            LastAccessed = lastAccessed;
            LastModified = lastModified;

            ContentHash = Convert.ToBase64String(contentHash);

            FileMetadata = fileMetadata ?? throw new ArgumentNullException(nameof(fileMetadata));
        }

        [JsonIgnore]
        public bool IsEmpty => ReferenceEquals(this, Empty);

        public UserFileMetadata? FileMetadata { get; }

        public string? ContentVersion { get; }
        public string? ContentType { get; }
        public string? ContentHash { get; }
        public ulong? ContentLength { get; }
        public string? ContentEncoding { get; }
        public string? ContentLanguage { get; }
        public DateTimeOffset? LastAccessed { get; }
        public DateTimeOffset LastModified { get; }
    }
}
