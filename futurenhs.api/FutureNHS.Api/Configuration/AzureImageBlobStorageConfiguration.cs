﻿namespace FutureNHS.Api.Configuration
{    
    public sealed class AzureImageBlobStorageConfiguration
    {
        public string ConnectionString { get; init; }
        public Uri? PrimaryServiceUrl { get; init; }
        public Uri? GeoRedundantServiceUrl { get; init; }
        public string ContainerName { get; init; }
    }
}
