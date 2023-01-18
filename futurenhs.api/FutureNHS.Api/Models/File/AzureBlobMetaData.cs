namespace FutureNHS.Api.Models.File;

public sealed record AzureBlobMetaData
{
    public byte[] ContentHash { get; init; }
    
    public string VersionId { get; init; }
}