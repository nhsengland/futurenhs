namespace Umbraco9ContentApi.Umbraco.AzurePlatform
{
    public sealed class AzureTableStorageConfiguration
    {
        public string ConnectionString { get; init; }
        public string TableName { get; init; }
    }
}
