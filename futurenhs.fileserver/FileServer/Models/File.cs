using System;
using System.Text.Json.Serialization;

namespace FileServer.Models
{
    // Not got a clue why this is so complicated but keeping it in incase it actually does something
    public readonly struct File : IEquatable<File>
    {
        internal const int FILENAME_MAXIMUM_LENGTH = 100;
        internal const int FILENAME_MINIMUM_LENGTH = 4;

        [JsonConstructor]

        public File(Guid id, string name, string? version)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            name = name.Trim();
            version = version?.Trim();

            if (FILENAME_MAXIMUM_LENGTH < name.Length) throw new ArgumentOutOfRangeException(nameof(name), $"Maximum allowed filename length is {FILENAME_MAXIMUM_LENGTH} characters");
            if (FILENAME_MINIMUM_LENGTH > name.Length) throw new ArgumentOutOfRangeException(nameof(name), $"Minimum allowed filename length is {FILENAME_MINIMUM_LENGTH} characters");
            
            Id = id;
            Name = name;
            Version = version;
        }
        
        public File(string name, string? version)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            name = name.Trim();
            version = version?.Trim();

            if (FILENAME_MAXIMUM_LENGTH < name.Length) throw new ArgumentOutOfRangeException(nameof(name), $"Maximum allowed filename length is {FILENAME_MAXIMUM_LENGTH} characters");
            if (FILENAME_MINIMUM_LENGTH > name.Length) throw new ArgumentOutOfRangeException(nameof(name), $"Minimum allowed filename length is {FILENAME_MINIMUM_LENGTH} characters");
            
            Id = Guid.Parse(name);
            Name = name;
            Version = version;
        }

        [JsonIgnore] public static File Empty { get; } = new File();
        [JsonIgnore] public Guid Id { get; }
        [JsonInclude] public string? Name { get; }
        [JsonInclude] public string? Version { get; }
        [JsonIgnore] public bool IsEmpty => string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Version);

        public static File With(string fileName, string fileVersion)
        {
            return new File(fileName, fileVersion);
        }

        public static File FromId(Guid id, string? fileVersion = default)
        {
            // The version of the file may be provided in the id, or alternatively in the header of the request if 
            // the WOPI client fully supported the versioning protocol.   The optional parameter is the value taken 
            // from the header if it exists.

            return new File(id.ToString(), fileVersion?.Trim());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + (Name ?? string.Empty).GetHashCode();
                hash = hash * 23 + (Version ?? string.Empty).GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (!typeof(File).IsAssignableFrom(obj.GetType())) return false;

            return Equals((File)obj);
        }

        public bool Equals(File other)
        {
            if (other.IsEmpty) return IsEmpty;

            if (0 != string.Compare(other.Name, Name, StringComparison.OrdinalIgnoreCase)) return false;
            if (0 != string.CompareOrdinal(other.Version, Version)) return false;

            return true;
        }

        public bool Equals(File? other)
        {
            if (other is null) return IsEmpty;

            return Equals(other.Value);
        }

        public static bool operator ==(File left, File right) => left.Equals(right);
        public static bool operator !=(File left, File right) => !left.Equals(right);

        public static implicit operator Guid(File file) => file.IsEmpty ? default : file.Id;
        public static implicit operator File(Guid id) => FromId(id);

        public override string ToString()
        {
            return $"File Name = '{Name ?? "null"}', File Version = '{Version ?? "null"}'";
        }
    }
}
