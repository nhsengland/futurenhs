using FutureNHS.Api.Services.Interfaces;

namespace FutureNHS.Api.Services
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IDiscussionService, DiscussionService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IGroupMembershipService, GroupMembershipService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupImageService, ImageService>();
            services.AddScoped<IUserImageService, ImageService>();
            services.AddScoped<IPermissionsService, PermissionsService>();

            return services;
        }
    }
}
