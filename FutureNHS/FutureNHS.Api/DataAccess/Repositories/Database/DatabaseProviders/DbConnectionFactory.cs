using System.Data;
using FutureNHS.Application.Interfaces;
using FutureNHS.Infrastructure.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Infrastructure.Repositories.Database.RetryPolicy;

namespace FutureNHS.Infrastructure.Repositories.Database.DatabaseProviders
{
    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDbRetryPolicy _retryPolicy;
        private readonly IApplicationSettings _applicationSettings;

        public DbConnectionFactory(IApplicationSettings applicationSettings, IDbRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
            _applicationSettings = applicationSettings;
        }

        public IDbConnection CreateReadOnlyConnection()
        {
            return new ReliableSqlDbConnection(_applicationSettings.ReadOnlyDbConnectionString, _retryPolicy);
        }

        public IDbConnection CreateWriteOnlyConnection()
        {
            return new ReliableSqlDbConnection(_applicationSettings.WriteOnlyDbConnectionString, _retryPolicy);
        }
    }
}
