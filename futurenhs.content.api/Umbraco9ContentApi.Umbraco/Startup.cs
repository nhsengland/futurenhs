using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Extensions;
using Umbraco9ContentApi.Core.Controllers;
using Umbraco9ContentApi.Core.Extensions;
using Umbraco9ContentApi.Umbraco.AzurePlatform;
using Umbraco9ContentApi.Umbraco.Providers.Logging;

namespace UmbracoContentApi.Umbraco
{


    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <remarks>
        /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337
        /// </remarks>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            var loggingTableStorageConfig = services.Configure<AzureTableStorageConfiguration> (_config.GetSection("Logging:TableStorageConfiguration"));
#pragma warning disable IDE0022 // Use expression body for methods
            services.AddUmbraco(_env, _config)
                .AddBackOffice()
                .AddWebsite()
                .AddComposers()
                .AddAzureBlobMediaFileSystem() // This configures the required services 
                .Build();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "FutureNhs Content API",
                    Description = "Application APIs for the FutureNhs Page Builder. Edit content using the Umbraco Backoffice.",
                    Version = "v1"
                });
                var filePath = Path.GetFullPath("~/../../Documentation/FutureNhsContentApi.xml");
                c.IncludeXmlComments(filePath);
            });

            services.Configure<UmbracoRenderingDefaultsOptions>(c =>
            {
                c.DefaultControllerType = typeof(DefaultRenderController);
            });
            services.AddApplicationInsightsTelemetry(_config["ApplicationInsights:ConnectionString"]);

            var loggingTableStorageConnectionString = _config["Logging:TableStorageConfiguration:ConnectionString"];
            var loggingTableStorageTableName = _config["Logging:TableStorageConfiguration:TableName"];
            
            services.AddSingleton<ILoggerProvider>(
                sp =>
                {
                    var config = sp.GetRequiredService<IOptionsMonitor<AzureTableStorageConfiguration>>();
                    if (config.CurrentValue is not null && !string.IsNullOrWhiteSpace(config.CurrentValue.TableName) && !string.IsNullOrWhiteSpace(config.CurrentValue.ConnectionString))
                    {
                        return new AzureTableLoggerProvider(config.CurrentValue.ConnectionString, config.CurrentValue.TableName);
                    }

                    return new EventLogLoggerProvider();

                });

            services.AddLogging();
#pragma warning restore IDE0022 // Use expression body for methods

        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseUmbraco()
                .WithMiddleware(u =>
                {
                    u.UseBackOffice();
                    u.UseWebsite();
                    u.UseAzureBlobMediaFileSystem();
                })
                .WithEndpoints(u =>
                {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                    u.UseWebsiteEndpoints();
                });

            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
