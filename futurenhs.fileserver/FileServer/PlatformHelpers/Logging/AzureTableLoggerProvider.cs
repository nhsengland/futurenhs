namespace FileServer.PlatformHelpers.Logging
{
    public class AzureTableLoggerProvider : ILoggerProvider
    {
        private readonly string _connectionStringName;
        private readonly string _tableName;

        public AzureTableLoggerProvider(string connectionString, string tableName)
        {
            _connectionStringName = connectionString;
            _tableName = tableName;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new AzureTableLogger(_connectionStringName, _tableName);
        }

        public void Dispose()
        {

        }
    }
}
