using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(MvcForum.Web.Startup))]
namespace MvcForum.Web
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Application.ViewEngine;
    using Core;
    using Core.Data.Context;
    using Core.Events;
    using Core.Interfaces;
    using Core.Ioc;
    using Core.Services.Migrations;
    using Core.Utilities;
    using Core.Interfaces.Services;
    using Core.Models.General;
    using Core.Reflection;
    using Core.Services;
    using Unity;
    using MvcForum.Core.Constants;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Start unity
            UnityHelper.InitialiseUnityContainer();

            // Make DB update to latest migration
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MvcForumContext, Configuration>());

            // Set the rest of the Ioc
            UnityHelper.BuildUnityContainer();

            // Grab the container as we will need to use it
            var unityContainer = UnityHelper.Container;

            // Set Hangfire to use SQL Server and the connection string
            GlobalConfiguration.Configuration.UseSqlServerStorage(ForumConfiguration.Instance.MvcForumContext);

            // Make hangfire use unity container
            GlobalConfiguration.Configuration.UseUnityActivator(unityContainer);

            // Add Hangfire
            // TODO - Do I need this dashboard?
            //app.UseHangfireDashboard();
            app.UseHangfireServer();

            // Get services needed
            var mvcForumContext = unityContainer.Resolve<IMvcForumContext>();
            var loggingService = unityContainer.Resolve<ILoggingService>();
            var assemblyProvider = unityContainer.Resolve<IAssemblyProvider>();
            var localizationService = unityContainer.Resolve<ILocalizationService>();
            var cacheService = unityContainer.Resolve<ICacheService>();
            ((IObjectContextAdapter)mvcForumContext).ObjectContext.Refresh(RefreshMode.StoreWins, mvcForumContext.LocaleStringResource);
            UpdateLanguages(mvcForumContext, localizationService);
        

            // Routes
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // If the same carry on as normal            
            var logFileSize = ForumConfiguration.Instance.LogFileMaxSizeBytes;
            loggingService.Initialise(logFileSize > 100000 ? logFileSize : 100000);
            loggingService.Error("START APP");

            // Find the plugin, pipeline and badge assemblies
            var assemblies = assemblyProvider.GetAssemblies(ForumConfiguration.Instance.PluginSearchLocations).ToList();
            ImplementationManager.SetAssemblies(assemblies);


            var theme = "Default";
            var settings = mvcForumContext.Setting.FirstOrDefault();
            if (settings != null)
            {
                theme = settings.Theme;
            }

            // Set the view engine
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ForumViewEngine(theme));

            // Initialise the events
            EventManager.Instance.Initialize(loggingService, assemblies);

            // Finally trigger any Cron jobs
            RecurringJob.AddOrUpdate<RecurringJobService>(x => x.SendMarkAsSolutionReminders(), Cron.HourInterval(6),
                queue: "solutionreminders");
        }

        private void UpdateLanguages(IMvcForumContext context, ILocalizationService localizationService)
        {
            var report = new CsvReport();
            try
            {
                // Now add the default language strings
                var file = HostingEnvironment.MapPath(@"~/Installer/en-GB.csv");
                var commaSeparator = new[] {','};

                // Unpack the data
                var allLines = new List<string>();
                if (file != null)
                {

                    using (var streamReader = new StreamReader(file, Encoding.UTF8, true))
                    {
                        while (streamReader.Peek() >= 0)
                        {
                            allLines.Add(streamReader.ReadLine());
                        }
                    }
                }

                // Read the CSV file and generate a language
                report = localizationService.FromCsv("en-GB", allLines);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                context.RollBack();
                report.Errors.Add(new CsvErrorWarning
                {
                    ErrorWarningType = CsvErrorWarningType.GeneralError,
                    Message = $"Unable to import language: {ex.Message}"
                });
            }
        }
    }
}