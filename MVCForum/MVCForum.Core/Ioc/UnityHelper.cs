namespace MvcForum.Core.Ioc
{
    using Data.Context;
    using Interfaces;
    using Interfaces.Services;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using MvcForum.Core.Factories;
    using MvcForum.Core.Interfaces.Factories;
    using MvcForum.Core.Interfaces.Helpers;
    using MvcForum.Core.Interfaces.Providers;
    using MvcForum.Core.Providers;
    using MvcForum.Core.Repositories.Command;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Database.DatabaseProviders;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Database.RetryPolicy;
    using MvcForum.Core.Repositories.Repository;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using MvcForum.Core.Utilities;
    using Reflection;
    using SendGrid;
    using Services;
    using System;
    using System.Configuration;
    using System.Web.Http;
    using System.Web.Mvc;
    using Unity;
    using Unity.Lifetime;


    /// <summary>
    ///     Bind the given interface in request scope
    /// </summary>
    public static class IocExtensions
    {
        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }
    }

    /// <summary>
    ///     The injection for Unity
    /// </summary>
    public static class UnityHelper
    {

        public static IUnityContainer Container;

        public static void InitialiseUnityContainer(HttpConfiguration config)
        {
            Container = new UnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(Container));
            config.DependencyResolver = new HttpDependencyResolver(Container);
            // Bit annoying having just this here but we need this early in the startup for seed method
            Container.BindInRequestScope<IConfigService, ConfigService>();
            Container.BindInRequestScope<ICacheService, CacheService>();
        }


        /// <summary>
        ///     Inject
        /// </summary>
        /// <returns></returns>
        public static void BuildUnityContainer()
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // Database context, one per request, ensure it is disposed
            Container.BindInRequestScope<IMvcForumContext, MvcForumContext>();

            //Bind the various domain model services and repositories that e.g. our controllers require         
            Container.BindInRequestScope<IRoleService, RoleService>();
            Container.BindInRequestScope<IGroupService, GroupService>();
            Container.BindInRequestScope<IMembershipService, MembershipService>();
            Container.BindInRequestScope<IPermissionService, PermissionService>();
            Container.BindInRequestScope<ISettingsService, SettingsService>();
            Container.BindInRequestScope<ITopicService, TopicService>();
            Container.BindInRequestScope<ITopicTagService, TopicTagService>();
            Container.BindInRequestScope<IPostService, PostService>();
            Container.BindInRequestScope<ILocalizationService, LocalizationService>();
            Container.BindInRequestScope<IVoteService, VoteService>();
            Container.BindInRequestScope<IMembershipUserPointsService, MembershipUserPointsService>();
            Container.BindInRequestScope<IGroupPermissionForRoleService, GroupPermissionForRoleService>();
            Container.BindInRequestScope<ILoggingService, LoggingService>();
            Container.BindInRequestScope<IEmailService, EmailService>();
            Container.BindInRequestScope<IReportService, ReportService>();
            Container.BindInRequestScope<IActivityService, ActivityService>();
            Container.BindInRequestScope<IPollService, PollService>();
            Container.BindInRequestScope<IBannedEmailService, BannedEmailService>();
            Container.BindInRequestScope<IBannedWordService, BannedWordService>();
            Container.BindInRequestScope<IUploadedFileService, UploadedFileService>();
            Container.BindInRequestScope<IFavouriteService, FavouriteService>();
            Container.BindInRequestScope<IGlobalPermissionForRoleService, GlobalPermissionForRoleService>();
            Container.BindInRequestScope<INotificationService, NotificationService>();
            Container.BindInRequestScope<IBlockService, BlockService>();
            Container.BindInRequestScope<IPostEditService, PostEditService>();
            Container.BindInRequestScope<IAssemblyProvider, AssemblyProvider>();
            Container.BindInRequestScope<ISpamService, SpamService>();
            Container.BindInRequestScope<IFeatureManager, FeatureManager>();
            Container.BindInRequestScope<IFolderCommand, FolderCommand>();
            Container.BindInRequestScope<IFolderService, FolderService>();
            Container.BindInRequestScope<IFileService, FileService>();
            Container.BindInRequestScope<IFileUploadValidationService, FileUploadValidationService>();
            Container.BindInRequestScope<IGroupInviteService, GroupInviteService>();
            Container.BindInRequestScope<IRegistrationEmailService, RegistrationEmailService>();
            Container.BindInRequestScope<ISmtpClientFactory, SmtpClientFactory>();
            Container.BindInRequestScope<IGroupAddMemberService, GroupAddMemberService>();
            Container.BindInRequestScope<IImageService, ImageService>();


            Container.RegisterSingleton<IValidateFileType, FileTypeValidator>();

            Container.RegisterInstance<IMemoryCache>(new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions())));

            switch (ConfigurationManager.AppSettings["SendEmailService"])
            {
                case "SendGrid":
                    Container.BindInRequestScope<ISendEmailService, SendGridEmailService>();
                    Container.RegisterInstance<ISendGridClient>(new SendGridClient(ConfigurationManager.AppSettings["Email_SendGridApiKey"]));
                    break;
                case "Smtp":
                    Container.BindInRequestScope<ISendEmailService, SmtpEmailService>();
                    break;
            }

            Container.BindInRequestScope<ISystemPagesService, SystemPagesService>();
            // Repositories
            Container.RegisterInstance<IConfigurationProvider>(new ConfigurationProvider(
                ConfigurationManager.ConnectionStrings["MVCForumContextReadOnly"].ConnectionString,
                ConfigurationManager.ConnectionStrings["MVCForumContext"].ConnectionString,
                Convert.ToInt32(ConfigurationManager.AppSettings["Polly_RetryAttempts"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["Polly_DelayBetweenAttempts"]),
                ConfigurationManager.ConnectionStrings["AzureBlobStorage:FilesPrimaryConnectionString_TO_BE_RETIRED"].ConnectionString,
                ConfigurationManager.AppSettings["AzureBlobStorage:FilesContainerName_TO_BE_RETIRED"],
                ConfigurationManager.AppSettings["AzureBlobStorage:FilesPrimaryEndpoint_TO_BE_RETIRED"],
                ConfigurationManager.AppSettings["Email_SmtpFrom"],
                ConfigurationManager.AppSettings["FileServer_TemplateUrl"],
                ConfigurationManager.AppSettings["FileServer_TemplateUrlFileIdPlaceholder"],
                ConfigurationManager.AppSettings["AzurePlatform:ApplicationGateway:FQDN"]));
            Container.BindInRequestScope<IDbRetryPolicy, DbRetryPolicy>();
            Container.BindInRequestScope<IDbConnectionFactory, DbConnectionFactory>();
            Container.BindInRequestScope<IFolderRepository, FolderRepository>();
            Container.BindInRequestScope<IFileRepository, FileRepository>();
            Container.BindInRequestScope<IFileCommand, FileCommand>();
            Container.BindInRequestScope<IGroupInviteRepository, GroupInviteRepository>();
            Container.BindInRequestScope<IGroupInviteCommand, GroupInviteCommand>();
            Container.BindInRequestScope<ISystemPagesRepository, SystemPagesRepository>();
            Container.BindInRequestScope<ISystemPagesCommand, SystemPagesCommand>();
            Container.BindInRequestScope<IGroupAddMemberRepository, GroupAddMemberRepository>();
            Container.BindInRequestScope<IGroupRepository, GroupRepository>();
            Container.BindInRequestScope<IGroupAddMemberCommand, GroupAddMemberCommand>();
            Container.BindInRequestScope<IGroupRepository, GroupRepository>();
            Container.BindInRequestScope<IGroupAddMemberCommand, GroupAddMemberCommand>();
            Container.BindInRequestScope<IGroupCommand, GroupCommand>();
            Container.BindInRequestScope<IFileServerService, FileServerService>();
            Container.BindInRequestScope<IUserRepository, UserRepository>();
            Container.BindInRequestScope<IImageRepository, ImageRepository>();
            Container.BindInRequestScope<IImageCommand, ImageCommand>();
        }
    }

    // Example of adding your own bindings, just create a partial class and implement
    // the CustomBindings method and add your bindings as shown below
    //public static partial class UnityHelper
    //{
    //    static partial void CustomBindings(UnityContainer container)
    //    {
    //        container.BindInRequestScope<IBlockRepository, BlockRepository>();
    //        container.BindInRequestScope<IBlockService, BlockService>();
    //    }
    //}
}
