using System.Data;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Providers.RetryPolicy;

namespace FutureNHS.Api.DataAccess.Database.Providers
{
    public sealed class AzureSqlDbConnectionFactory : IAzureSqlDbConnectionFactory
    {
        private readonly string _readWriteConnectionString;
        private readonly string _readOnlyConnectionString;
        private readonly IDbRetryPolicy _dbRetryPolicy;
        private readonly ILogger<AzureSqlDbConnectionFactory>? _logger;

        public AzureSqlDbConnectionFactory(string readWriteConnectionString, string readOnlyConnectionString, IDbRetryPolicy dbRetryPolicy, ILogger<AzureSqlDbConnectionFactory>? logger)
        {
            if (string.IsNullOrWhiteSpace(readWriteConnectionString)) throw new ArgumentNullException(nameof(readWriteConnectionString));
            if (string.IsNullOrWhiteSpace(readOnlyConnectionString)) throw new ArgumentNullException(nameof(readOnlyConnectionString));

            _logger = logger;

            _readWriteConnectionString = readWriteConnectionString;
            _readOnlyConnectionString = readOnlyConnectionString;
            _dbRetryPolicy = dbRetryPolicy;
        }

        ValueTask<IDbConnection> ISqlDbConnectionFactory<IDbConnection>.GetReadOnlyConnectionAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sqlConnection =  new ReliableSqlDbConnection(_readOnlyConnectionString, _dbRetryPolicy);

            if (sqlConnection is null) throw new FailedToConnectToSqlDatabaseException("The ReadOnly Sql Connection could not be established. Cause unknown but possibly tied to the provision of an invalid read only connection string?");

            return new ValueTask<IDbConnection>(sqlConnection);
        }

        ValueTask<IDbConnection> ISqlDbConnectionFactory<IDbConnection>.GetReadWriteConnectionAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sqlConnection = new ReliableSqlDbConnection(_readWriteConnectionString, _dbRetryPolicy);

            if (sqlConnection is null) throw new FailedToConnectToSqlDatabaseException("The Read/Write Sql Connection could not be established. Cause unknown but possibly tied to the provision of an invalid read write connection string?");

            return new ValueTask<IDbConnection>(sqlConnection);
        }
    }
}
