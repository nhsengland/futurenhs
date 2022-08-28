using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost.Azure
{
    public interface IAzureSqlClient
    {
        Task<T?> GetRecord<T>(string sqlQuery, object parameters, CancellationToken cancellationToken) where T : class;
    }

    public sealed class AzureSqlClient : IAzureSqlClient
    {
        private readonly static ConcurrentDictionary<string, AsyncPolicyWrap> _globalPolicies = new();

        private readonly IAzureSqlDbConnectionFactory _azureSqlDbConnectionFactory;
        private readonly ILogger<AzureSqlClient>? _logger;

        public AzureSqlClient(IAzureSqlDbConnectionFactory azureSqlDbConnectionFactory, ILogger<AzureSqlClient>? logger)
        {
            _logger = logger;

            _azureSqlDbConnectionFactory = azureSqlDbConnectionFactory ?? throw new ArgumentNullException(nameof(azureSqlDbConnectionFactory));
        }

        async Task<T?> IAzureSqlClient.GetRecord<T>(string sqlQuery, object parameters, CancellationToken cancellationToken)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery)) throw new ArgumentNullException(nameof(sqlQuery));

            cancellationToken.ThrowIfCancellationRequested();

            using var sqlConnection = await _azureSqlDbConnectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var cmd = new CommandDefinition(sqlQuery, parameters, cancellationToken: cancellationToken);

            var retryPolicy = GetAsyncRetryPolicy();

            var globalResiliencyPolicy = GetAsyncGlobalResiliencyPolicyFor(sqlConnection.ConnectionString);

            var resiliencyPolicy = Policy.WrapAsync(retryPolicy, globalResiliencyPolicy);

            var record = await resiliencyPolicy.ExecuteAsync(ct => sqlConnection.QuerySingleOrDefaultAsync<T>(cmd), cancellationToken);

            // TODO - Possible leaky abstraction if we let things like SqlException escalate?   Extend interface to expose custom exception types for common failures
            //        such as failing to connect to the remote store?

            return record;
        }


#if DEBUG
        internal
#else
        private
#endif
            AsyncPolicyWrap GetAsyncGlobalResiliencyPolicyFor(string policyKey)
        {
            if (_globalPolicies.TryGetValue(policyKey, out var cachedPolicy)) return cachedPolicy;

            var bulkheadPolicy = GetAsyncBulkheadPolicy();

            var circuitBreakerPolicy = GetAsyncCircuitBreakerPolicy();

            var newPolicy = Policy.WrapAsync(bulkheadPolicy, circuitBreakerPolicy);

            _globalPolicies.AddOrUpdate(policyKey, newPolicy, (_, oldPolicy) => newPolicy = oldPolicy);

            return newPolicy;
        }

        [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Understand this is an internal API and risks of future incompatibility")]
#if DEBUG
        internal
#else
        private
#endif
        AsyncPolicy GetAsyncRetryPolicy()
        {
            const int RETRY_ATTEMPTS_ON_TRANSIENT_ERROR = 5;

            var jitterer = new Random();

            var retryPolicyWithJitter =
                Policy.Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn).
                       Or<TimeoutException>().
                       Or<TimeoutRejectedException>().
                       OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn).
                       WaitAndRetryAsync(
                          retryCount: RETRY_ATTEMPTS_ON_TRANSIENT_ERROR,
                          sleepDurationProvider: retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)),
                          (ex, sleepingFor, retryNumber, ctxt) =>
                          {
                              _logger?.LogTrace("Azure SQL Retry handler on iteration {RetryNumber} sleeping {SleepDuration} ms after error '{ErrorMsg}' against context with correlation id = '{CorrelationId}'", retryNumber, sleepingFor.TotalMilliseconds, ex.Message, ctxt.CorrelationId);
                          }
                      );

            return retryPolicyWithJitter;
        }

#if DEBUG
        internal
#else
        private
#endif
        AsyncPolicy GetAsyncBulkheadPolicy() => Policy.BulkheadAsync(
            maxParallelization: 3,  // At most 3 concurrent requests to the database at the same time
            maxQueuingActions: 25,  // If we have more than 3 concurrent requests, queue up to a maximum of 25 other requests before we start to refuse them
            ctxt =>
            {
                _logger?.LogTrace("Azure SQL Bulkhead Policy rejected request with correlation id = '{CorrelationId}'", ctxt.CorrelationId);

                return Task.CompletedTask;
            }
            );


        [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
#if DEBUG
        internal
#else
        private
#endif
        static AsyncPolicy GetAsyncCircuitBreakerPolicy() => 
            Policy.Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn).
                   Or<TimeoutException>().
                   Or<TimeoutRejectedException>().
                   OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn).
                   AdvancedCircuitBreakerAsync(
                      failureThreshold: 0.25,                             // If 25% or more of requests fail
                      samplingDuration: TimeSpan.FromSeconds(60),   // in a 60 second period
                      minimumThroughput: 7,                               // and there have been at least 7 requests in that time
                      durationOfBreak: TimeSpan.FromSeconds(30)     // then open the circuit for 30 seconds
                      );
    }
}
