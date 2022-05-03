using Microsoft.Extensions.Logging;
using Umbraco9ContentApi.Umbraco.Providers.Logging;

namespace Umbraco9ContentApi.Core.Extensions
{
    public static class AzureTableLoggerExtensions
    {
        public static ILoggerFactory AddTableStorage(this ILoggerFactory loggerFactory, string tableName, string connectionString)
        {
            loggerFactory.AddProvider(new AzureTableLoggerProvider(connectionString, tableName));
            return loggerFactory;
        }
    }
}
