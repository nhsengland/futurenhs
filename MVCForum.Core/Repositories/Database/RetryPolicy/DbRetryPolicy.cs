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


            
            //const int FIFTEEN_SECONDS = 15;
            //const int RETRY_ATTEMPTS_ON_TRANSIENT_ERROR = 3;
            //var jitterer = new Random();
            //var retryPolicyWithJitter = Policy.Handle<SqlException>(exception => _sqlExceptions.Contains(exception.Number)).
            //    WaitAndRetryAsync(
            //        retryCount: RETRY_ATTEMPTS_ON_TRANSIENT_ERROR,
            //        sleepDurationProvider: retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
            //    );
            //var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(FIFTEEN_SECONDS);
            //var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization: 3, maxQueuingActions: 25);
            //var circuitBreakerPolicy =
            //    Policy.HandleResult<HttpResponseMessage>(rm => !rm.IsSuccessStatusCode).
            //        AdvancedCircuitBreakerAsync(
            //            failureThreshold: 0.25,                             // If 25% or more of requests fail
            //            samplingDuration: TimeSpan.FromSeconds(60),         // in a 60 second period
            //            minimumThroughput: 7,                               // and there have been at least 7 requests in that time
            //            durationOfBreak: TimeSpan.FromSeconds(30)           // then open the circuit for 30 seconds
            //        );
            //builder.AddPolicyHandler(timeoutPolicy).
            //    AddPolicyHandler(circuitBreakerPolicy).
            //    AddPolicyHandler(bulkheadPolicy).
            //    AddPolicyHandler(retryPolicyWithJitter);
        }

        public void Execute(Action operation)
        {
            _retryPolicy.Execute(operation.Invoke);
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return _retryPolicy.Execute(operation.Invoke);
        }

        public async Task Execute(Func<Task> operation, CancellationToken cancellationToken)
        {
            await _retryPolicyAsync.ExecuteAsync(operation.Invoke);
        }

        public async Task<TResult> Execute<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            return await _retryPolicyAsync.ExecuteAsync(operation.Invoke);
        }
    }
}
