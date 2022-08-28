using Azure;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FutureNHS-WOPI-Host-UnitTests")]

namespace FutureNHS.WOPIHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == StatusCodes.Status429TooManyRequests)
                {
                    // This can happen when the azure app configuration service is throttling requests, although it takes a long time to 
                    // actually throw the error from the SDK (think this might be because the retry-after response can be up to 24 hours)

                    // Think the best we can do is allow the application to bootstrap using the base configuration and thus potentially degrade
                    // but then there would be no way to gracefully recover when the app config service comes back online while this app service
                    // is still running

                    // Will defer for now and come back when we have a better plan, on the assumption we will be moving away from the free tier
                    // of app config and thus far less likely to hit the problem (1000 daily limit on free, 20000 on standard)

                    throw;
                }
            }
        }

        // Example of how to dynamically handle changes to app config without restarting the application
        // https://docs.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-aspnet-core?tabs=core3x 
        
        // Some information on high availability setup for app config service
        // https://docs.microsoft.com/en-us/azure/azure-app-configuration/concept-disaster-recovery

        // TODO - Remove support for connection string usage and force Azure Identity to be provided if running locally as well as in Azure

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var settings = config.Build();

                        // We want to use the application's managed identity (when hosted in Azure) to connect to the configuration service 
                        // If running locally and your AAD account doesn't have access to it, populate the AzurePlatform:AzureAppConfiguration:PrimaryConnectionString and optionally 
                        // the AzurePlatform:AzureAppConfiguration:GeoRedundantReadOnlyConnectionString (for multi region failover)
                        // configuration values and it will connect using that method instead, noting you only need to use read-only keys

                        var credential = new DefaultAzureCredential();

                        // We will pull down our app configuration from the Azure Configuration Service, noting that we first pull down 
                        // all configuration without a label, and then override some/all of those setting with those labelled with the 
                        // value held in the ASPNETCORE_ENVIRONMENT variable (production, development etc).
                        // This should allow us to easily manage different config (including feature flags) as we move between environments, 
                        // but unfortunately, if you are using secrets.json locally and the EnvironmentName variable isn't set to 'Development', the secrets will 
                        // not be imported, so either you can't use secrets.json or your env label has to be Development (ie not dev, prod etc)

                        // For added resilience in a multi-region configuration, we can add a secondary endpoint to retrieve configuration 
                        // from just in case the primary is not available.  Hardly ideal, but ACS doesn't support geo-failover so until it 
                        // does we have to do the best we can and try to keep settings in sync ourselves :(

                        if (bool.TryParse(Environment.GetEnvironmentVariable("USE_AZURE_APP_CONFIGURATION"), out var useAppConfig) && useAppConfig)
                        {
                            // NB - If the App Configuration Service is being throttled when we start up the application, this method does not appear to ever complete
                            //      which stops the startup class from bootstrapping the application which then sits in a zombie state until Azure recycles (and round we go).
                            //      It appears to be a flaw in the Microsoft Extensions and I've been unable to figure out if there is a way to cancel the operation and 
                            //      fall back to using the local configuration settings. 

                            var geoRedundantReadOnlyConnectionString = settings.GetConnectionString("AzurePlatform:AzureAppConfiguration:GeoRedundantReadOnlyConnectionString");
                            var geoRedundantServiceUrl = settings["AzurePlatform:AzureAppConfiguration:GeoRedundantServiceUrl"];

                            var isMultiRegion = !string.IsNullOrWhiteSpace(geoRedundantReadOnlyConnectionString) || Uri.IsWellFormedUriString(geoRedundantServiceUrl, UriKind.Absolute);

                            var environmentLabel = hostingContext.HostingEnvironment.EnvironmentName;

                            var refreshSchedule = settings.GetSection("AzurePlatform:AzureAppConfiguration").GetValue("CacheExpirationIntervalInSeconds", defaultValue: 60 * 5);

                            var cacheExpirationInterval = refreshSchedule >= 1 ? TimeSpan.FromSeconds(refreshSchedule) : TimeSpan.FromMinutes(5);

                            if (isMultiRegion)
                            {
                                config.AddAzureAppConfiguration(
                                    options =>
                                    {
                                        // If the connection string is specified in the configuration, use that instead of relying on a 
                                        // managed identity (which may not work in a local dev environment)

                                        if (!string.IsNullOrWhiteSpace(geoRedundantReadOnlyConnectionString))
                                        {
                                            options = options.Connect(geoRedundantReadOnlyConnectionString);
                                        }
                                        else
                                        {
                                            options = options.Connect(new Uri(geoRedundantServiceUrl, UriKind.Absolute), credential);
                                        }

                                        options.Select(keyFilter: KeyFilter.Any, labelFilter: LabelFilter.Null)
                                               .Select(keyFilter: KeyFilter.Any, labelFilter: environmentLabel)
                                               .ConfigureRefresh(refreshOptions => refreshOptions.Register("FileServer_SentinelKey", refreshAll: true))
                                               .ConfigureKeyVault(kv => kv.SetCredential(credential))
                                               .UseFeatureFlags(featureFlagOptions => featureFlagOptions.CacheExpirationInterval = cacheExpirationInterval);
                                    },
                                    optional: true
                                    );
                            }

                            var primaryConnectionString = settings.GetConnectionString("AzureAppConfiguration:PrimaryConnectionString");
                            var primaryServiceUrl = settings["AzurePlatform:AzureAppConfiguration:PrimaryServiceUrl"];

                            config.AddAzureAppConfiguration(
                                options =>
                                {
                                    // If the connection string is specified in the configuration, use that instead of relying on a 
                                    // managed identity (which may not work in a local dev environment)

                                    if (!string.IsNullOrWhiteSpace(primaryConnectionString))
                                    {
                                        options = options.Connect(primaryConnectionString);
                                    }
                                    else if (Uri.IsWellFormedUriString(primaryServiceUrl, UriKind.Absolute))
                                    {
                                        options = options.Connect(new Uri(primaryServiceUrl, UriKind.Absolute), credential);
                                    }
                                    else throw new ApplicationException("If the USE_AZURE_APP_CONFIGURATION environment variable is set to true then either the ConnectionStrings:AzureAppConfiguration-Primary or the AzureAppConfiguration:PrimaryEndpoint setting must be present and well formed");

                                    options.Select(keyFilter: KeyFilter.Any, labelFilter: LabelFilter.Null)
                                           .Select(keyFilter: KeyFilter.Any, labelFilter: environmentLabel)
                                           .ConfigureRefresh(refreshOptions => refreshOptions.Register("FileServer_SentinelKey", refreshAll: true)
                                                                                             .SetCacheExpiration(cacheExpirationInterval))
                                           .ConfigureKeyVault(kv => kv.SetCredential(credential))
                                           .UseFeatureFlags(featureFlagOptions => featureFlagOptions.CacheExpirationInterval = cacheExpirationInterval);
                                },
                                optional: isMultiRegion
                                );
                        }
                    })
                .UseStartup<Startup>());
    }
}
