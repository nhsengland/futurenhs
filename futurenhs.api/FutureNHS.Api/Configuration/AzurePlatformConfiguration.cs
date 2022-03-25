namespace FutureNHS.Api.Configuration
{
    public sealed class AzurePlatformConfiguration
    {
        public AzureAppConfiguration? AzureAppConfiguration { get; set; }
        public AzureSqlConfiguration? AzureSql { get; set; }
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
