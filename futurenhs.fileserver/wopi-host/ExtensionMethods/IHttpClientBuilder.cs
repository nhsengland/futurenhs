using Microsoft.Extensions.DependencyInjection;
using System;

namespace FutureNHS.WOPIHost
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Tasked with extending the http message handling pipeline for outbound requests to wrap it with the core 
        /// retry, circuit breaker and bulkhead isolation policies applicable to named http clients
        /// </summary>
        /// <param name="httpClientBuilder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddCoreResiliencyPolicies(this IHttpClientBuilder httpClientBuilder)
        {
            if (httpClientBuilder is null) throw new ArgumentNullException(nameof(httpClientBuilder));

            return httpClientBuilder.AddHttpMessageHandler<CoreResilientRetryHandler>();
        }
    }
}
