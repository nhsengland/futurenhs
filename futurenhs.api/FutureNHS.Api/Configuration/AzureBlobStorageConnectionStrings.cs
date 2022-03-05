namespace FutureNHS.Api.Configuration
{    
    public sealed class AzureBlobStorageConnectionStrings
    {
        public string FilePrimaryConnectionString { get; init; }
        public string ImagePrimaryConnectionString { get; init; }
    }
}
