using System.Data;
using System.Data.Common;
using FileServer.DataAccess.Sql.RetryPolicy;
using Microsoft.Data.SqlClient;

namespace FileServer.PlatformHelpers
{
    public class ReliableSqlDbCommand : DbCommand
    {
        private readonly SqlCommand _underlyingSqlCommand;
        private readonly IDbRetryPolicy _retryPolicy;

        public ReliableSqlDbCommand(SqlCommand command, IDbRetryPolicy retryPolicy)
        {
            _underlyingSqlCommand = command;
            _retryPolicy = retryPolicy;
        }

        public override string CommandText
        {
            get => _underlyingSqlCommand.CommandText;
            set => _underlyingSqlCommand.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => _underlyingSqlCommand.CommandTimeout;
            set => _underlyingSqlCommand.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => _underlyingSqlCommand.CommandType;
            set => _underlyingSqlCommand.CommandType = value;
        }

        public IDbConnection? Connection { get; set; }
        public IDataParameterCollection Parameters { get; }
        public IDbTransaction? Transaction { get; set; }

        public override bool DesignTimeVisible
        {
            get => _underlyingSqlCommand.DesignTimeVisible;
            set => _underlyingSqlCommand.DesignTimeVisible = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => _underlyingSqlCommand.UpdatedRowSource;
            set => _underlyingSqlCommand.UpdatedRowSource = value;
        }

        protected override DbConnection DbConnection
        {
            get => _underlyingSqlCommand.Connection;
            set => _underlyingSqlCommand.Connection = (SqlConnection)value;
        }

        protected override DbParameterCollection DbParameterCollection => _underlyingSqlCommand.Parameters;

        protected override DbTransaction DbTransaction
        {
            get => _underlyingSqlCommand.Transaction;
            set => _underlyingSqlCommand.Transaction = (SqlTransaction)value;
        }

        public override void Cancel()
        {
            _underlyingSqlCommand.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            return _retryPolicy.RetryPolicy.Execute(() => _underlyingSqlCommand.ExecuteNonQuery());
        }

        public override object ExecuteScalar()
        {
            return _retryPolicy.RetryPolicy.Execute(() => _underlyingSqlCommand.ExecuteScalar());
        }
        public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return _retryPolicy.RetryPolicyAsync.ExecuteAsync(() => _underlyingSqlCommand.ExecuteScalarAsync(cancellationToken));
        }

        public override void Prepare()
        {
            _retryPolicy.RetryPolicy.Execute(() => _underlyingSqlCommand.Prepare());
        }

        protected override DbParameter CreateDbParameter()
        {
            return _underlyingSqlCommand.CreateParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return _retryPolicy.RetryPolicy.Execute(() => _underlyingSqlCommand.ExecuteReader(behavior));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _underlyingSqlCommand.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
