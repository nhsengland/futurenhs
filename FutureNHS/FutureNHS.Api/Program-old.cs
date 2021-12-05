//using System.Collections.Immutable;
//using Azure.Identity;
//using FutureNHS.Api.Configuration;
//using Microsoft.ApplicationInsights.DependencyCollector;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging.ApplicationInsights;
//using Microsoft.FeatureManagement;
//using FutureNHS.Api.Models.Pagination.Services;
//using FutureNHS.Application.Application;
//using FutureNHS.Application.Interfaces;
//using FutureNHS.Infrastructure;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Configuration.AzureAppConfiguration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Internal;
//using Microsoft.Extensions.Options;

//var builder = WebApplication.CreateBuilder(args);

//// Providing an instrumentation key is required if you're using the
//// standalone Microsoft.Extensions.Logging.ApplicationInsights package,
//// or when you need to capture logs during application startup, such as
//// in Program.cs or Startup.cs itself.
//builder.Logging.AddApplicationInsights(builder.Configuration["ApplicationInsights:ConnectionString"]);

//// Capture all log-level entries from Program
//builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(
//    typeof(Program).FullName, LogLevel.Trace);

//// Add services to the container.
//builder.Services.AddSingleton<IApplicationSettings>((s) =>
//    new ApplicationSettings(
//        builder.Configuration.GetConnectionString("MVCForumDBReadOnly"),
//        builder.Configuration.GetConnectionString("MVCForumDB"),
//        builder.Configuration.GetValue<int>("Polly:RetryCount"),
//        builder.Configuration.GetValue<int>("Polly:RetryDelay")
//    ));
//builder.Services.AddControllers();
//builder.Services.AddApiVersioning(config =>
//{
//    config.DefaultApiVersion = new ApiVersion(1, 0);
//    config.AssumeDefaultVersionWhenUnspecified = true;
//    config.ReportApiVersions = true;
//});
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<IUriService>(o =>
//{
//    var accessor = o.GetRequiredService<IHttpContextAccessor>();
//    var request = accessor.HttpContext?.Request;
//    var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
//    return new UriService(uri);

//});

//builder.Services.AddInfrastructure();


//// Add Azure Application Insights
//var appInsightsInstrumentationKey = builder.Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");

//if (!string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
//{
//    builder.Services.AddApplicationInsightsTelemetry(appInsightsInstrumentationKey);

//    builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
//        (module, o) =>
//        {
//            module.EnableSqlCommandTextInstrumentation = true;
//        });
//}


//builder.Services.Configure<Features>(builder.Configuration.GetSection("FeatureManagement"), binderOptions => binderOptions.BindNonPublicProperties = true);
//builder.Services.Configure<AzurePlatformConfiguration>(builder.Configuration.GetSection("AzurePlatform"));

//builder.Services.AddSingleton<ISystemClock>(new SystemClock());

//var azureAppConfig = new ConfigurationBuilder()
//      .AddAzureAppConfiguration(options =>
//      {
//          options.Connect(builder.Configuration.GetConnectionString("AppConfig")).UseFeatureFlags();
//      })
//      .Build();

//builder.Services.AddFeatureManagement(azureAppConfig);


//var primaryConnectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration:PrimaryConnectionString");
//var primaryServiceUrl = builder.Configuration.GetValue["AzurePlatform:AzureAppConfiguration:PrimaryServiceUrl"];


//var settings = builder.Build();

//// We want to use the application's managed identity (when hosted in Azure) to connect to the configuration service 
//// If running locally and your AAD account doesn't have access to it, populate the AzurePlatform:AzureAppConfiguration:PrimaryConnectionString and optionally 
//// the AzurePlatform:AzureAppConfiguration:GeoRedundantReadOnlyConnectionString (for multi region failover)
//// configuration values and it will connect using that method instead, noting you only need to use read-only keys

//var credential = new DefaultAzureCredential();

//// We will pull down our app configuration from the Azure Configuration Service, noting that we first pull down 
//// all configuration without a label, and then override some/all of those setting with those labelled with the 
//// value held in the ASPNETCORE_ENVIRONMENT variable (production, development etc).
//// This should allow us to easily manage different config (including feature flags) as we move between environments, 
//// but unfortunately, if you are using secrets.json locally and the EnvironmentName variable isn't set to 'Development', the secrets will 
//// not be imported, so either you can't use secrets.json or your env label has to be Development (ie not dev, prod etc)

//// For added resilience in a multi-region configuration, we can add a secondary endpoint to retrieve configuration 
//// from just in case the primary is not available.  Hardly ideal, but ACS doesn't support geo-failover so until it 
//// does we have to do the best we can and try to keep settings in sync ourselves :(

//if (bool.TryParse(Environment.GetEnvironmentVariable("USE_AZURE_APP_CONFIGURATION"), out var useAppConfig) && useAppConfig)
//{
//    // NB - If the App Configuration Service is being throttled when we start up the application, this method does not appear to ever complete
//    //      which stops the startup class from bootstrapping the application which then sits in a zombie state until Azure recycles (and round we go).
//    //      It appears to be a flaw in the Microsoft Extensions and I've been unable to figure out if there is a way to cancel the operation and 
//    //      fall back to using the local configuration settings. 

//    var geoRedundantReadOnlyConnectionString = builder.Configuration.GetConnectionString("AzurePlatform:AzureAppConfiguration:GeoRedundantReadOnlyConnectionString");
//    var geoRedundantServiceUrl = builder.Configuration["AzurePlatform:AzureAppConfiguration:GeoRedundantServiceUrl"];

//    var isMultiRegion = !string.IsNullOrWhiteSpace(geoRedundantReadOnlyConnectionString) || Uri.IsWellFormedUriString(geoRedundantServiceUrl, UriKind.Absolute);

//    var environmentLabel = builder.Environment.EnvironmentName;

//    var refreshSchedule = builder.Configuration.GetSection("AzurePlatform:AzureAppConfiguration").GetValue("CacheExpirationIntervalInSeconds", defaultValue: 60 * 5);

//    var cacheExpirationInterval = refreshSchedule >= 1 ? TimeSpan.FromSeconds(refreshSchedule) : TimeSpan.FromMinutes(5);

//    if (isMultiRegion)
//    {
//        builder.Configuration.AddAzureAppConfiguration(
//            options =>
//            {
//                // If the connection string is specified in the configuration, use that instead of relying on a 
//                // managed identity (which may not work in a local dev environment)

//                if (!string.IsNullOrWhiteSpace(geoRedundantReadOnlyConnectionString))
//                {
//                    options = options.Connect(geoRedundantReadOnlyConnectionString);
//                }
//                else
//                {
//                    options = options.Connect(new Uri(geoRedundantServiceUrl, UriKind.Absolute), credential);
//                }

//                options.Select(keyFilter: KeyFilter.Any, labelFilter: LabelFilter.Null)
//                       .Select(keyFilter: KeyFilter.Any, labelFilter: environmentLabel)
//                       .ConfigureRefresh(refreshOptions => refreshOptions.Register("FileServer_SentinelKey", refreshAll: true))
//                       .ConfigureKeyVault(kv => kv.SetCredential(credential))
//                       .UseFeatureFlags(featureFlagOptions => featureFlagOptions.CacheExpirationInterval = cacheExpirationInterval);
//            },
//            optional: true
//            );
//    }

//    var primaryConnectionString = settings.GetConnectionString("AzureAppConfiguration:PrimaryConnectionString");
//    var primaryServiceUrl = settings["AzurePlatform:AzureAppConfiguration:PrimaryServiceUrl"];

//    config.AddAzureAppConfiguration(
//        options =>
//        {
//            // If the connection string is specified in the configuration, use that instead of relying on a 
//            // managed identity (which may not work in a local dev environment)

//            if (!string.IsNullOrWhiteSpace(primaryConnectionString))
//            {
//                options = options.Connect(primaryConnectionString);
//            }
//            else if (Uri.IsWellFormedUriString(primaryServiceUrl, UriKind.Absolute))
//            {
//                options = options.Connect(new Uri(primaryServiceUrl, UriKind.Absolute), credential);
//            }
//            else throw new ApplicationException("If the USE_AZURE_APP_CONFIGURATION environment variable is set to true then either the ConnectionStrings:AzureAppConfiguration-Primary or the AzureAppConfiguration:PrimaryEndpoint setting must be present and well formed");

//            options.Select(keyFilter: KeyFilter.Any, labelFilter: LabelFilter.Null)
//                   .Select(keyFilter: KeyFilter.Any, labelFilter: environmentLabel)
//                   .ConfigureRefresh(refreshOptions => refreshOptions.Register("FileServer_SentinelKey", refreshAll: true)
//                                                                     .SetCacheExpiration(cacheExpirationInterval))
//                   .ConfigureKeyVault(kv => kv.SetCredential(credential))
//                   .UseFeatureFlags(featureFlagOptions => featureFlagOptions.CacheExpirationInterval = cacheExpirationInterval);
//        },
//        optional: isMultiRegion
//        );
//}
//var app = builder.Build();




//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
