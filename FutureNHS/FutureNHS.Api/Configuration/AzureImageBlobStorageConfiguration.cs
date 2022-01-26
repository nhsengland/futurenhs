namespace FutureNHS.Api.Configuration
{    
    public sealed class AzureImageBlobStorageConfiguration
    {
        public string PrimaryServiceUrl { get; init; }
        public string GeoRedundantServiceUrl { get; init; }
        public string ContainerName { get; init; }
    }
}
