using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class HealthCheckDataProvider : IHealthCheckDataProvider
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<HealthCheckDataProvider> _logger;

        public HealthCheckDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<HealthCheckDataProvider> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<bool> CheckDatabaseConnectionAsync(CancellationToken cancellationToken = default)
        {
            const string query = @"SELECT (1)";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            return await dbConnection.QuerySingleAsync<bool>(query);
        }
    }
}
