//-----------------------------------------------------------------------
// <copyright file="DbRetryPolicy.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Database.RetryPolicy
{
    using Polly;
    using Polly.Retry;
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MvcForum.Core.Interfaces.Providers;

    public class DbRetryPolicy : IDbRetryPolicy
    {
        // Only retry on connection errors
        private readonly int[] _sqlExceptions = new[] { 53, -2 };

        private readonly AsyncRetryPolicy _retryPolicyAsync;
        private readonly Policy _retryPolicy;

        public DbRetryPolicy(IConfigurationProvider configurationProvider)
        {
            var retryCount = configurationProvider.GetRetryAttempts();
            var waitBetweenRetriesInMilliseconds = configurationProvider.GetRetryDelay();

            _retryPolicyAsync = Policy
                .Handle<SqlException>(exception => _sqlExceptions.Contains(exception.Number))
                .WaitAndRetryAsync(
                    retryCount: retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(waitBetweenRetriesInMilliseconds)
                );

            _retryPolicy = Policy
                .Handle<SqlException>(exception => _sqlExceptions.Contains(exception.Number))
                .WaitAndRetry(
                    retryCount: retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(waitBetweenRetriesInMilliseconds)
                );
        }

        public void Execute(Action operation)
        {
            _retryPolicy.Execute(operation.Invoke);
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return _retryPolicy.Execute(operation.Invoke);
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
        {
            await _retryPolicyAsync.ExecuteAsync(operation.Invoke, cancellationToken);
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken)
        {
            return await _retryPolicyAsync.ExecuteAsync(operation.Invoke, cancellationToken);
        }
    }
}
