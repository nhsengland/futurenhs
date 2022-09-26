using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    /// <summary>
    /// Core message handler that adds circuit breaker, bulkhead and optionally retry policies to an HttpRequest (for HttpMethod.Get)
    /// </summary>
    public sealed class CoreResilientRetryHandler : DelegatingHandler
    {
        const int RETRY_ATTEMPTS_ON_TRANSIENT_ERROR = 3;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            var jitterer = new Random();

            var retryPolicyWithJitter = HttpPolicyExtensions.
                HandleTransientHttpError().
                WaitAndRetryAsync(
                    retryCount: RETRY_ATTEMPTS_ON_TRANSIENT_ERROR,
                    sleepDurationProvider: retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                    );

            var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization: 3, maxQueuingActions: 25);

            var circuitBreakerPolicy =
                Policy.HandleResult<HttpResponseMessage>(rm => !rm.IsSuccessStatusCode).
                       AdvancedCircuitBreakerAsync(
                            failureThreshold: 0.25,                             // If 25% or more of requests fail
                            samplingDuration: TimeSpan.FromSeconds(60),         // in a 60 second period
                            minimumThroughput: 7,                               // and there have been at least 7 requests in that time
                            durationOfBreak: TimeSpan.FromSeconds(30)           // then open the circuit for 30 seconds
                            );

            var policy = HttpMethod.Get == httpRequestMessage.Method
                ? Policy.WrapAsync(retryPolicyWithJitter, bulkheadPolicy, circuitBreakerPolicy) 
                : Policy.WrapAsync(bulkheadPolicy, circuitBreakerPolicy);

            return policy.ExecuteAsync(() => base.SendAsync(httpRequestMessage, cancellationToken));
        }
    }
}
