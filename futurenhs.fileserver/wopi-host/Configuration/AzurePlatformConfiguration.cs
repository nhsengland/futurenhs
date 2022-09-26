using System;

namespace FutureNHS.WOPIHost.Configuration
{
    public sealed class AzurePlatformConfiguration
    {
        public AzureBlobStorageConfiguration? AzureBlobStorage { get; set; }
        public AzureTableStorageConfiguration? AzureTableStorage { get; set; }
        public AzureAppConfiguration? AzureAppConfiguration { get; set; }
        public AzureSqlConfiguration? AzureSql { get; set; }
    }

    public sealed class AzureBlobStorageConfiguration
    {
        public Uri? PrimaryServiceUrl { get; set; }
        public Uri? GeoRedundantServiceUrl { get; set; }
        public string? ContainerName { get; set; }
    }

    public sealed class AzureTableStorageConfiguration
    {
        public Uri? PrimaryServiceUrl { get; set; }
        public Uri? GeoRedundantServiceUrl { get; set; }
        public string? AccessTokenTableName { get; set; }
    }

    public sealed class AzureAppConfiguration
    {
        public int? CacheExpirationIntervalInSeconds { get; set; }

        public Uri? PrimaryServiceUrl { get; set; }
        public Uri? GeoRedundantServiceUrl { get; set; }
    }

    public sealed class AzureSqlConfiguration
    {
        public string? ReadWriteConnectionString { get; set; }
        public string? ReadOnlyConnectionString { get; set; }
    }
}
