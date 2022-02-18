using FutureNHS.Api.Services.Interfaces;

namespace FutureNHS.Api.Services
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IDiscussionService, DiscussionService>();
            services.AddScoped<IGroupMembershipService, GroupMembershipService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IPermissionsService, PermissionsService>();

            return services;
        }
    }
}
