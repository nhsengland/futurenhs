using Polly;
using Polly.Retry;
using Microsoft.Data.SqlClient;
using FutureNHS.Application.Interfaces;
using Polly.CircuitBreaker;
using Polly.Wrap;

namespace FutureNHS.Infrastructure.Repositories.Database.RetryPolicy
{
    public class DbRetryPolicy : IDbRetryPolicy
    {
        const int RETRY_ATTEMPTS_ON_TRANSIENT_ERROR = 3;

        // Only retry on connection errors
        private readonly int[] _sqlExceptions = new[] { 53, -2 };

        public DbRetryPolicy()
        {
            var jitterer = new Random();

            var retryPolicyWithJitterAsync = Policy.Handle<SqlException>(exception => _sqlExceptions.Contains(exception.Number)).
                WaitAndRetryAsync(
                    retryCount: RETRY_ATTEMPTS_ON_TRANSIENT_ERROR,
                    sleepDurationProvider: retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );
        
            var bulkheadPolicyAsync = Policy.BulkheadAsync(maxParallelization: 3, maxQueuingActions: 25);

            var circuitBreakerPolicyAsync = Policy
                .Handle<SqlException>(exception => _sqlExceptions.Contains(exception.Number)).
                AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.25,                             // If 25% or more of requests fail
                    samplingDuration: TimeSpan.FromSeconds(60),         // in a 60 second period
                    minimumThroughput: 7,                               // and there have been at least 7 requests in that time
                    durationOfBreak: TimeSpan.FromSeconds(30)           // then open the circuit for 30 seconds
                );

            RetryPolicyAsync = Policy.WrapAsync(retryPolicyWithJitterAsync, bulkheadPolicyAsync, circuitBreakerPolicyAsync);


            var bulkheadPolicy = Policy.Bulkhead(maxParallelization: 3, maxQueuingActions: 25);

            var retryPolicyWithJitter = Policy.Handle<SqlException>(exception => _sqlExceptions.Contains(exception.Number)).
                WaitAndRetry(
                    retryCount: RETRY_ATTEMPTS_ON_TRANSIENT_ERROR,
                    sleepDurationProvider: retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );


            var circuitBreakerPolicy = Policy
                .Handle<SqlException>(exception => _sqlExceptions.Contains(exception.Number)).
            AdvancedCircuitBreaker(
                failureThreshold: 0.25,                             // If 25% or more of requests fail
                samplingDuration: TimeSpan.FromSeconds(60),         // in a 60 second period
                minimumThroughput: 7,                               // and there have been at least 7 requests in that time
                durationOfBreak: TimeSpan.FromSeconds(30)           // then open the circuit for 30 seconds
            );


            RetryPolicy = Policy.Wrap(retryPolicyWithJitter, bulkheadPolicy, circuitBreakerPolicy);
        }

        public AsyncPolicyWrap RetryPolicyAsync { get; }
        public PolicyWrap RetryPolicy { get; }
    }

        //public void Execute(Action operation)
        //{
        //    _policy.Execute(operation.Invoke);
        //}

        //public TResult Execute<TResult>(Func<TResult> operation)
        //{
        //    return _policy.Execute(operation.Invoke);
        //}

        //public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
        //{
        //    await _policyAsync.ExecuteAsync(() => operation.Invoke(cancellationToken));
        //}

        //public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken)
        //{
        //    return _policyAsync.ExecuteAsync(() => operation.Invoke(cancellationToken));
        //}

        //public async Task<TResult> ExecuteAsync<TResult>(Task operation, CancellationToken cancellationToken)
        //{

        //    return await _policyAsync.ExecuteAsync(() => operation);
        //}


    }
