using FutureNHS.Api.Services.Admin;
using FutureNHS.Api.Services.Admin.Interfaces;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Notifications;
using FutureNHS.Api.Services.Notifications.Interfaces;

namespace FutureNHS.Api.Services
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<IDiscussionService, DiscussionService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileServerService, FileServerService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IGroupImageService, ImageService>();
            services.AddScoped<IGroupMembershipService, GroupMembershipService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserImageService, ImageService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IFileServerService, FileServerService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<ICommentNotificationService, CommentNotificationService>();
            services.AddScoped<IGroupMemberNotificationService, GroupMemberNotificationService>();

            services.AddScoped<IAdminGroupService, AdminGroupService>();
            services.AddScoped<IAdminUserService, AdminUserService>(); 
            services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();

            return services;
        }
    }
}
