namespace FileServer.Models;

public sealed record AzureBlobMetadata
{
    public byte[] ContentHash { get; init; }
    
    public string VersionId { get; init; }
    }