using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost.Azure
{
    public interface IAzureSqlDbConnectionFactory : ISqlDbConnectionFactory<IDbConnection>
    {

    }

    public sealed class AzureSqlDbConnectionFactory : IAzureSqlDbConnectionFactory
    {
        private readonly string _readWriteConnectionString;
        private readonly string _readOnlyConnectionString;
        private readonly ILogger<AzureSqlDbConnectionFactory>? _logger;

        public AzureSqlDbConnectionFactory(string readWriteConnectionString, string readOnlyConnectionString, ILogger<AzureSqlDbConnectionFactory>? logger)
        {
            if (string.IsNullOrWhiteSpace(readWriteConnectionString)) throw new ArgumentNullException(nameof(readWriteConnectionString));
            if (string.IsNullOrWhiteSpace(readOnlyConnectionString)) throw new ArgumentNullException(nameof(readOnlyConnectionString));

            _logger = logger;

            _readWriteConnectionString = readWriteConnectionString;
            _readOnlyConnectionString = readOnlyConnectionString;
        }

        ValueTask<IDbConnection> ISqlDbConnectionFactory<IDbConnection>.GetReadOnlyConnectionAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sqlConnection = new SqlConnection(_readOnlyConnectionString, credential: default);

            if (sqlConnection is null) throw new FailedToConnectToSqlDatabaseException("The ReadOnly Sql Connection could not be established. Cause unknown but possibly tied to the provision of an invalid read only connection string?");

            return new ValueTask<IDbConnection>(sqlConnection);
        }

        ValueTask<IDbConnection> ISqlDbConnectionFactory<IDbConnection>.GetReadWriteConnectionAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sqlConnection = new SqlConnection(_readWriteConnectionString, credential: default);

            if (sqlConnection is null) throw new FailedToConnectToSqlDatabaseException("The Read/Write Sql Connection could not be established. Cause unknown but possibly tied to the provision of an invalid read write connection string?");

            return new ValueTask<IDbConnection>(sqlConnection);
        }
    }
}
