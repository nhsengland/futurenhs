//-----------------------------------------------------------------------
// <copyright file="ReliableSqlDbConnection.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Database.DatabaseProviders
{
    using Polly.Retry;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using MvcForum.Core.Interfaces.Providers;
    using MvcForum.Core.Repositories.Database.RetryPolicy;

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

        public override void Open()
        {
            _retryPolicy.Execute(() =>
            {
                if (_underlyingConnection.State != ConnectionState.Open)
                {
                    _underlyingConnection.Open();
                }
            });
        }

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            var y = cancellationToken;
            if (_underlyingConnection.State != ConnectionState.Open)
            {
                await _retryPolicy.ExecuteAsync(ct => _underlyingConnection.OpenAsync(ct), cancellationToken);
            }
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
