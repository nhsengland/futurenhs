namespace MvcForum.Core.Repositories.Database.DatabaseProviders
{
    using Polly.Retry;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using MvcForum.Core.Repositories.Database.RetryPolicy;

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

        public override int ExecuteNonQuery()
        {
            return _retryPolicy.Execute(() => _underlyingSqlCommand.ExecuteNonQuery());
        }

        public override object ExecuteScalar()
        {
            return _retryPolicy.Execute(() => _underlyingSqlCommand.ExecuteScalar());
        }

        public override void Prepare()
        {
            _retryPolicy.Execute(() => _underlyingSqlCommand.Prepare());
        }

        protected override DbParameter CreateDbParameter()
        {
            return _underlyingSqlCommand.CreateParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return _retryPolicy.Execute(() => _underlyingSqlCommand.ExecuteReader(behavior));
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
