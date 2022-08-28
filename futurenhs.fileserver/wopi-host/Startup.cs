using FutureNHS.WOPIHost.Azure;
using FutureNHS.WOPIHost.Configuration;
using FutureNHS.WOPIHost.WOPIRequests;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FutureNHS.WOPIHost
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("wopi-discovery-document").AddCoreResiliencyPolicies();
            services.AddHttpClient("mvcforum-userinfo").AddCoreResiliencyPolicies();

            services.AddMemoryCache(); //options => { });

            var appInsightsInstrumentationKey = _configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");

            if (!string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
            {
                services.AddApplicationInsightsTelemetry(appInsightsInstrumentationKey);

                services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
                    (module, o) => { 
                        module.EnableSqlCommandTextInstrumentation = true; 
                    });
            }
            else services.AddLogging();

            services.AddHttpContextAccessor();

            if (bool.TryParse(Environment.GetEnvironmentVariable("USE_AZURE_APP_CONFIGURATION"), out var useAppConfig) && useAppConfig)
            {
                services.AddAzureAppConfiguration();
            }

            services.Configure<Features>(_configuration.GetSection("FeatureManagement"), binderOptions => binderOptions.BindNonPublicProperties = true);
            services.Configure<WopiConfiguration>(_configuration.GetSection("Wopi"));
            services.Configure<AzurePlatformConfiguration>(_configuration.GetSection("AzurePlatform"));
            services.Configure<AppConfiguration>(_configuration.GetSection("App"));

            services.AddSingleton<ISystemClock>(new SystemClock());

            services.AddScoped<CoreResilientRetryHandler>();

            services.AddScoped<IAzureBlobStoreClient, AzureBlobStoreClient>();
            services.AddScoped<IAzureTableStoreClient, AzureTableStoreClient>();

            //services.AddScoped<IAzureBlobStoreClient>(
            //    sp => {
            //        var config = sp.GetRequiredService<IOptionsSnapshot<AzurePlatformConfiguration>>().Value.AzureBlobStorage;

            //        if (config is null) throw new ApplicationException("Unable to load the azure blob storage configuration");
            //        if (config.PrimaryServiceUrl is null) throw new ApplicationException("The azure blob storage primary service url is null in the files configuration section");
            //        if (config.GeoRedundantServiceUrl is null) throw new ApplicationException("The azure blob storage geo-redundant service url is null in the files configuration section");

            //        var memoryCache = sp.GetRequiredService<IMemoryCache>();
            //        var clock = sp.GetRequiredService<ISystemClock>();
            //        var logger = sp.GetRequiredService<ILogger<AzureBlobStoreClient>>();

            //        return new AzureBlobStoreClient(config.PrimaryServiceUrl, config.GeoRedundantServiceUrl, memoryCache, clock, logger);
            //    });

            services.AddScoped<IAzureSqlDbConnectionFactory>(
                sp => {
                    var config = sp.GetRequiredService<IOptionsSnapshot<AzurePlatformConfiguration>>().Value.AzureSql;

                    if (config is null) throw new ApplicationException("Unable to load the azure sql configuration");
                    if (string.IsNullOrWhiteSpace(config.ReadWriteConnectionString)) throw new ApplicationException("The azure read write connection string is missing from the files configuration section");
                    if (string.IsNullOrWhiteSpace(config.ReadOnlyConnectionString)) throw new ApplicationException("The azure read only connection string is missing from the files configuration section");
 
                    var logger = sp.GetRequiredService<ILogger<AzureSqlDbConnectionFactory>>();

                    return new AzureSqlDbConnectionFactory(config.ReadWriteConnectionString, config.ReadOnlyConnectionString, logger);
                    });

            services.AddScoped<IAzureSqlClient, AzureSqlClient>();

            services.AddScoped<IFileContentMetadataRepository, FileContentMetadataRepository>();
            
            services.AddScoped<IUserFileMetadataProvider, UserFileMetadataProvider>();
            services.AddScoped<IUserFileAccessTokenRepository, UserFileAccessTokenRepository>();

            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();

            services.AddScoped<IWopiDiscoveryDocumentFactory, WopiDiscoveryDocumentFactory>();
            services.AddScoped<IWopiDiscoveryDocumentRepository, WopiDiscoveryDocumentRepository>();
            services.AddScoped<IWopiRequestHandlerFactory, WopiRequestHandlerFactory>();
            services.AddScoped<IWopiCryptoProofChecker, WopiCryptoProofChecker>();

            services.AddFeatureManagement().
                     AddFeatureFilter<TimeWindowFilter>().      // enable a feature between a start and end date ....... https://docs.microsoft.com/dotnet/api/microsoft.featuremanagement.featurefilters.timewindowfilter?view=azure-dotnet-preview
                     AddFeatureFilter<PercentageFilter>();      // for randomly sampling a percentage of the audience .. https://docs.microsoft.com/dotnet/api/microsoft.featuremanagement.featurefilters.percentagefilter?view=azure-dotnet-preview
                   //AddFeatureFilter<TargetingFilter>();       // for targeting certain audiences ..................... https://docs.microsoft.com/dotnet/api/microsoft.featuremanagement.featurefilters.targetingfilter?view=azure-dotnet-preview
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            if (bool.TryParse(Environment.GetEnvironmentVariable("USE_AZURE_APP_CONFIGURATION"), out var useAppConfig) && useAppConfig)
            {
                app.UseAzureAppConfiguration();
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseRouting();
            //app.UseRequestLocalization();
            //app.UseCors();
            //app.UseAuthentication();
            //app.UseAuthorization();
            //app.UseSession();
            //app.UseResponseCompression();
            //app.UseResponseCaching();

            app.UseMiddleware<WopiMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/wopi/health-check", GetHealthCheckPageAsync);
                endpoints.MapGet("/wopi/collabora",  GetCollaboraHostPageAsync);
           });
        }

        private static async Task GetHealthCheckPageAsync(HttpContext httpContext)
        {
            // https://docs.microsoft.com/en-us/azure/app-service/monitor-instances-health-check
            // check the user agent string to ensure this endpoint is secured inside Azure?



            //await httpContext.Response.WriteAsync(sb.ToString());
        }

        /// <summary>
        /// This is here purely as an example of how a host page needs to be rendered for it to be able to first post to Collabora and then have it 
        /// relay back to this WOPI host to serve up and manage the actual file
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static async Task GetCollaboraHostPageAsync(HttpContext httpContext)
        {
            var cancellationToken = httpContext.RequestAborted;

            var fileId = httpContext.Request.Query["file_id"].FirstOrDefault()?.Trim();
            var userId = httpContext.Request.Query["user_id"].FirstOrDefault()?.Trim();

            const string LATEST_FILE_VERSION = "";
            const string FILENAME_ON_CDSDEV = "E399A9B2-2783-41F0-AC9B-ADA60087FF21";
            const string USERID_ON_CDSDEV = "EFB2F7F4-8E54-4B01-A634-AD3A00EDC7D6";

            if (string.IsNullOrWhiteSpace(fileId)) fileId = File.With(FILENAME_ON_CDSDEV, LATEST_FILE_VERSION); // "DF796179-DB2F-4A06-B4D5-AD7F012CC2CC", "2021-08-09T18:15:02.4214747Z");
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out _)) userId = USERID_ON_CDSDEV; 

            Debug.Assert(fileId is not null);

            var file = File.FromId(fileId);

            var authenticatedUser = new AuthenticatedUser(Guid.Parse(userId), "An Example User");

            var postAuthRequestHandler = AuthoriseUserRequestHandler.With(authenticatedUser, FileAccessPermission.View, file);

            var fakeHttpContext = new DefaultHttpContext() { 
                RequestServices = httpContext.RequestServices,
            };

            using var responseBodyStream = new MemoryStream();

            fakeHttpContext.Response.Body = responseBodyStream;

            await postAuthRequestHandler.HandleAsync(fakeHttpContext, cancellationToken);

            if (fakeHttpContext.Response.StatusCode != 200) throw new ApplicationException("Unable to get the user auth data");

            Debug.Assert(responseBodyStream is not null);

            responseBodyStream.Position = 0;

            var responseBody = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(responseBodyStream, cancellationToken: cancellationToken);

            Debug.Assert(responseBody is not null);

            var accessToken = responseBody["accessToken"];

            var collaboraOnlineEndpoint = responseBody["wopiClientUrlForFile"];


            var sb = new StringBuilder();

            // https://wopi.readthedocs.io/projects/wopirest/en/latest/concepts.html#term-wopisrc

            // When running locally in DEBUG ...
            // https://127.0.0.1:9980/loleaflet/4aa2794/loleaflet.html? is the Collabora endpoint for this document type, pulled out of the discovery xml file hosted by Collabora
            // https://127.0.0.1:44355/wopi/files/<FILE_ID> is the url Collabora uses to callback to us to get the file information and contents

            // In Azure, the container will be mapped to port 80 in docker run command

            // TODO - Generate a token with a set TTL that is specific to the current user and file combination
            //        This token will be sent back to us by Collabora by way of it verifying the request (it will be signed so we know it 
            //        comes from them and hasn't been tampered with outside of the servers)
            //        For now, we'll just use a Guid

            //var accessToken = Guid.NewGuid().ToString().Replace("-", string.Empty);

            // TODO - This is either going to have to be generated by MVCForum or somehow injected by it after a call to our API,
            //        but given the need for input elements, it might be more appropriate for us to just generate the token and 
            //        return both it and the collabora endpoint that needs to be used, or MVCForum gets the discovery document itself
            //        and generates a token we can later understand

            httpContext.Response.StatusCode = StatusCodes.Status200OK;

            sb.AppendLine($"<!doctype html>");
            sb.AppendLine($"<html>");
            sb.AppendLine($"  <body>");

            sb.AppendLine($"    <form action=\"{collaboraOnlineEndpoint}\" enctype =\"multipart/form-data\" method=\"post\">");
            sb.AppendLine($"      <input name=\"access_token\" value=\"{ accessToken }\" type=\"hidden\">");
            sb.AppendLine($"      <input type=\"submit\" value=\"View Document\">");
            sb.AppendLine($"    </form>");

            sb.AppendLine($"    <form action=\"{collaboraOnlineEndpoint}\" enctype =\"multipart/form-data\" method=\"post\" target=\"collabora_host_frame\">");
            sb.AppendLine($"      <input name=\"access_token\" value=\"{ accessToken }\" type=\"hidden\">");
            sb.AppendLine($"      <input type=\"submit\" value=\"View Document in iFrame\">");
            sb.AppendLine($"    </form>");

            sb.AppendLine($"    <form action=\"{collaboraOnlineEndpoint}\" enctype =\"multipart/form-data\" method=\"post\" target=\"_blank\">");
            sb.AppendLine($"      <input name=\"access_token\" value=\"{ accessToken }\" type=\"hidden\">");
            sb.AppendLine($"      <input type=\"submit\" value=\"View Document in new Window\">");
            sb.AppendLine($"    </form>");

            sb.AppendLine($"    <iframe name=\"collabora_host_frame\" allowfullscreen width=\"500px\" height=\"750px\">");
            sb.AppendLine($"    </iframe>");

            sb.AppendLine($"  </body>");
            sb.AppendLine($"</html>");

            await httpContext.Response.WriteAsync(sb.ToString());
        }
    }
}
