using System.Data;
using System.Data.Common;
using FileServer.DataAccess.Sql.RetryPolicy;
using Microsoft.Data.SqlClient;

namespace FileServer.PlatformHelpers
{
    public class ReliableSqlDbConnection : DbConnection
    {
        private readonly SqlConnection _underlyingConnection;
        private readonly IDbRetryPolicy _retryPolicy;

        private string _connectionString;

        public ReliableSqlDbConnection(string connectionString, IDbRetryPolicy retryPolicy)
        {
            _connectionString = connectionString;
            _retryPolicy = retryPolicy;
            _underlyingConnection = new SqlConnection(connectionString);
        }

        public override string ConnectionString
        {
            get => _connectionString;

            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Value cannot be null or empty.", nameof(value));
                _connectionString = value;
                _underlyingConnection.ConnectionString = value;
            }
        }

        public override string Database => _underlyingConnection.Database;

        public override string DataSource => _underlyingConnection.DataSource;

        public override string ServerVersion => _underlyingConnection.ServerVersion;

        public override ConnectionState State => _underlyingConnection.State;

        public override void ChangeDatabase(string databaseName)
        {
            _underlyingConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _underlyingConnection.Close();
        }

        public override async Task CloseAsync()
        {
            await _retryPolicy.RetryPolicyAsync.ExecuteAsync(() => _underlyingConnection.State != ConnectionState.Closed ? _underlyingConnection.CloseAsync() : null);
        }

        public override void Open()
        {
            _retryPolicy.RetryPolicy.Execute(() =>
            {
                if (_underlyingConnection.State != ConnectionState.Open)
                {
                    _underlyingConnection.Open();
                }
            });
        }



        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            await _retryPolicy.RetryPolicyAsync.ExecuteAsync(() => _underlyingConnection.State != ConnectionState.Open ? 
                _underlyingConnection.OpenAsync(cancellationToken) : null);
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _underlyingConnection.BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new ReliableSqlDbCommand(_underlyingConnection.CreateCommand(), _retryPolicy);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_underlyingConnection.State == ConnectionState.Open)
                {
                    _underlyingConnection.Close();
                }

                _underlyingConnection.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
