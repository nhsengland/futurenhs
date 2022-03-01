using FutureNHS.Api.DataAccess.Database.Read;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;

namespace FutureNHS.Api.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection DataAccess(this IServiceCollection services)
        {
            // Read
            services.AddScoped<IGroupDataProvider, GroupDataProvider>();
            services.AddScoped<IImageDataProvider, ImageDataProvider>();
            services.AddScoped<IHealthCheckDataProvider, HealthCheckDataProvider>();
            services.AddScoped<IRolesDataProvider, RolesDataProvider>();
            services.AddScoped<IPermissionsDataProvider, PermissionsDataProvider>();
            services.AddScoped<IFileAndFolderDataProvider, FileAndFolderDataProvider>();
            services.AddScoped<ISearchDataProvider, SearchDataProvider>();
            services.AddScoped<IDiscussionDataProvider, DiscussionDataProvider>();
            services.AddScoped<ICommentsDataProvider, CommentsDataProvider>();
            services.AddScoped<ISystemPageDataProvider, SystemPageDataProvider>();

            // Write
            services.AddScoped<IDiscussionCommand, DiscussionCommand>();
            services.AddScoped<IGroupCommand, GroupCommand>();
            services.AddScoped<IFileCommand, FileCommand>();
            services.AddScoped<IFolderCommand, FolderCommand>();
            services.AddScoped<IRolesCommand, RolesCommand>();
            services.AddScoped<ICommentCommand, CommentCommand>();

            return services;
        }
    }
}
