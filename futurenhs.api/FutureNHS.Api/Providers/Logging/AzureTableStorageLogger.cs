using Azure;
using Azure.Data.Tables;

namespace FutureNHS.Api.Providers.Logging
{


    public class AzureTableLogger : ILogger
    {
        private readonly TableClient _tableClient;
        public AzureTableLogger(string connectionString, string tableName)
        {
            var cloudTableClient = new TableClient(connectionString, tableName);
            cloudTableClient.CreateIfNotExistsAsync();
            _tableClient = cloudTableClient;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = new LogEntity
            {
                EventId = eventId.ToString(),
                LogLevel = logLevel.ToString(),
                Message = formatter(state, exception),//exception?.ToString(),
                PartitionKey = DateTime.Now.ToString("yyyyMMdd"),
                RowKey = Guid.NewGuid().ToString()
            };

            _tableClient.AddEntityAsync(log);
        }
    }

    public class LogEntity : ITableEntity
    {
        public string LogLevel { get; set; }
        public string EventId { get; set; }
        public string Message { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
