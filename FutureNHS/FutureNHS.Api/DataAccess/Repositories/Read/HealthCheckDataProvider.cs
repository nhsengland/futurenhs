using Dapper;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Application.Application.HardCodedSettings;
using FutureNHS.Application.Interfaces;
using FutureNHS.Infrastructure.Models;
using FutureNHS.Infrastructure.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Infrastructure.Repositories.Read.Interfaces;

namespace FutureNHS.Infrastructure.Repositories.Read
{
    public class HealthCheckDataProvider : IHealthCheckDataProvider
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;

        public HealthCheckDataProvider(IAzureSqlDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CheckDatabaseConnectionAsync(CancellationToken cancellationToken = default)
        {
            const string query = @"SELECT (1)";
            bool connected;
            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
               connected = await dbConnection.QuerySingleAsync<bool>(query);
            }

            return connected;
        }
    }
}
