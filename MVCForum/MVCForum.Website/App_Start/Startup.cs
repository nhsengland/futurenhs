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
    using Core.Interfaces.Services;
    using Core.Models.General;
    using Core.Reflection;
    using Core.Services;
    using Unity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security.OAuth;
    using MvcForum.Core.Constants;
    using MvcForum.Web.Application.Providers;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Start unity
            UnityHelper.InitialiseUnityContainer(System.Web.Http.GlobalConfiguration.Configuration);

            // Make DB update to latest migration
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<MvcForumContext, Configuration>());

            // Set the rest of the Ioc
            UnityHelper.BuildUnityContainer();

            // Grab the container as we will need to use it
            var unityContainer = UnityHelper.Container;

            // Set Hangfire to use SQL Server and the connection string
            //GlobalConfiguration.Configuration.UseSqlServerStorage(ForumConfiguration.Instance.MvcForumContext);

            // Make hangfire use unity container
            //GlobalConfiguration.Configuration.UseUnityActivator(unityContainer);

            // Configure OWIN
            app.CreatePerOwinContext<MvcForumContext>(() => new MvcForumContext());
            app.CreatePerOwinContext<UserManager<IdentityUser>>(CreateManager);
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth/token"),
                Provider = new AuthorizationProvider(new UnityDependencyResolver(unityContainer).GetService<IMembershipService>()),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                AllowInsecureHttp = true,

            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            // Add Hangfire
            // TODO - Do I need this dashboard?
            //app.UseHangfireDashboard();
            //app.UseHangfireServer();

            // Get services needed
            var mvcForumContext = unityContainer.Resolve<IMvcForumContext>();
            var loggingService = unityContainer.Resolve<ILoggingService>();
            var assemblyProvider = unityContainer.Resolve<IAssemblyProvider>();
            var localizationService = unityContainer.Resolve<ILocalizationService>();
            var cacheService = unityContainer.Resolve<ICacheService>();
            ((IObjectContextAdapter)mvcForumContext).ObjectContext.Refresh(RefreshMode.StoreWins, mvcForumContext.LocaleStringResource);


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
            //RecurringJob.AddOrUpdate<RecurringJobService>(x => x.SendMarkAsSolutionReminders(), Cron.HourInterval(6),
            //    queue: "solutionreminders");
        }

        private static UserManager<IdentityUser> CreateManager(IdentityFactoryOptions<UserManager<IdentityUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>(context.Get<MvcForumContext>());
            var owinManager = new UserManager<IdentityUser>(userStore);
            return owinManager;
        }
    }
}