using AspNetCore.Authentication.ApiKey;
using Azure.Identity;
using FutureNHS.Api.Authorization;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders;
using FutureNHS.Api.DataAccess.Repositories.Database.DatabaseProviders.Interfaces;
using FutureNHS.Api.Services;
using FutureNHS.Infrastructure.Repositories.Database.RetryPolicy;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

var settings = builder.Configuration;

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

if (bool.TryParse(settings.GetValue<string>("USE_AZURE_APP_CONFIGURATION"), out var useAppConfig) && useAppConfig)
{
    // NB - If the App Configuration Service is being throttled when we start up the application, this method does not appear to ever complete
    //      which stops the startup class from bootstrapping the application which then sits in a zombie state until Azure recycles (and round we go).
    //      It appears to be a flaw in the Microsoft Extensions and I've been unable to figure out if there is a way to cancel the operation and 
    //      fall back to using the local configuration settings. 

    var geoRedundantReadOnlyConnectionString = settings.GetConnectionString("AzurePlatform:AzureAppConfiguration:GeoRedundantReadOnlyConnectionString");
    var geoRedundantServiceUrl = settings["AzurePlatform:AzureAppConfiguration:GeoRedundantServiceUrl"];

    var isMultiRegion = !string.IsNullOrWhiteSpace(geoRedundantReadOnlyConnectionString) || Uri.IsWellFormedUriString(geoRedundantServiceUrl, UriKind.Absolute);

    var environmentLabel = builder.Environment.EnvironmentName;

    var refreshSchedule = settings.GetSection("AzurePlatform:AzureAppConfiguration").GetValue("CacheExpirationIntervalInSeconds", defaultValue: 60 * 5);

    var cacheExpirationInterval = refreshSchedule >= 1 ? TimeSpan.FromSeconds(refreshSchedule) : TimeSpan.FromMinutes(5);

    if (isMultiRegion)
    {
        settings.AddAzureAppConfiguration(
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

    settings.AddAzureAppConfiguration(
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

var appInsightsInstrumentationKey = settings.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");

if (!string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
{
    builder.Services.AddApplicationInsightsTelemetry(appInsightsInstrumentationKey);

    builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
        (module, o) => {
            module.EnableSqlCommandTextInstrumentation = true;
        });
}
else builder.Services.AddLogging();

builder.Services.AddHttpContextAccessor();

if (useAppConfig)
{
    builder.Services.AddAzureAppConfiguration();
}

builder.Services.Configure<Features>(settings.GetSection("FeatureManagement"), binderOptions => binderOptions.BindNonPublicProperties = true);
builder.Services.Configure<AzurePlatformConfiguration>(settings.GetSection("AzurePlatform"));
builder.Services.Configure<SharedSecrets>(settings.GetSection("SharedSecrets"));
builder.Services.Configure<AzureImageBlobStorageConfiguration>(settings.GetSection("AzurePlatform:AzureImageBlobStorage"));

builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddScoped<IDbRetryPolicy, DbRetryPolicy>();
builder.Services.AddScoped<IAzureSqlDbConnectionFactory>(
    sp => {
        var config = sp.GetRequiredService<IOptionsSnapshot<AzurePlatformConfiguration>>().Value.AzureSql;

        if (config is null) throw new ApplicationException("Unable to load the azure sql configuration");
        if (string.IsNullOrWhiteSpace(config.ReadWriteConnectionString)) throw new ApplicationException("The azure read write connection string is missing from the files configuration section");
        if (string.IsNullOrWhiteSpace(config.ReadOnlyConnectionString)) throw new ApplicationException("The azure read only connection string is missing from the files configuration section");

        var logger = sp.GetRequiredService<ILogger<AzureSqlDbConnectionFactory>>();

        return new AzureSqlDbConnectionFactory(config.ReadWriteConnectionString, config.ReadOnlyConnectionString, sp.GetRequiredService<IDbRetryPolicy>(), logger);
    });

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

builder.Services.DataAccess();
builder.Services.Services();

// It requires Realm to be set in the options if SuppressWWWAuthenticateHeader is not set.
// If an implementation of IApiKeyProvider interface is used as well as options.Events.OnValidateKey delegate is also set then this delegate will be used first.

builder.Services.AddScoped<IApiKeyRepository>(
    sp => {
        var config = sp.GetRequiredService<IOptionsSnapshot<SharedSecrets>>().Value;

        if (config is null) throw new ApplicationException("Unable to load the azure sql configuration");
        if (string.IsNullOrWhiteSpace(config.WebApplication)) throw new ApplicationException("The Web Application Key is missing from the Shared secrets configuration section");
        if (string.IsNullOrWhiteSpace(config.Owner)) throw new ApplicationException("The Owner is missing from the Shared secrets configuration section");

        var logger = sp.GetRequiredService<ILogger<IApiKeyRepository>>();

        return new ApiKeyRepository(config.WebApplication, config.Owner, logger);
    });

builder.Services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)

    // The below AddApiKeyInHeaderOrQueryParams without type parameter will require options.Events.OnValidateKey delegete to be set.
    //.AddApiKeyInHeaderOrQueryParams(options =>

    // The below AddApiKeyInHeaderOrQueryParams with type parameter will add the ApiKeyProvider to the dependency container. 
    .AddApiKeyInAuthorizationHeader<ApiKeyProvider>(options =>
    {
        options.Realm = "FutureNHS";
        options.KeyName = "Bearer";
    });

//// By default, authentication is not challenged for every request which is ASP.NET Core's default intended behaviour.
//// So to challenge authentication for every requests please use below FallbackPolicy option.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();    // NOTE: DEFAULT TEMPLATE DOES NOT HAVE THIS, THIS LINE IS REQUIRED AND HAS TO BE ADDED!!!
app.UseAuthorization();

app.MapControllers();

app.Run();
