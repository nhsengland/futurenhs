namespace FutureNHS.Api.Configuration
{    
    public sealed class AzureTableStorageConfiguration
    {
        public string ConnectionString { get; init; }
        public string TableName { get; init; }
    }
}
