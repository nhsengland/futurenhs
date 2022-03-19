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
            services.AddScoped<ICommentsDataProvider, CommentsDataProvider>();
            services.AddScoped<IDiscussionDataProvider, DiscussionDataProvider>();
            services.AddScoped<IFileAndFolderDataProvider, FileAndFolderDataProvider>();
            services.AddScoped<IGroupDataProvider, GroupDataProvider>();
            services.AddScoped<IHealthCheckDataProvider, HealthCheckDataProvider>();
            services.AddScoped<IImageDataProvider, ImageDataProvider>();
            services.AddScoped<IPermissionsDataProvider, PermissionsDataProvider>();
            services.AddScoped<IRolesDataProvider, RolesDataProvider>();
            services.AddScoped<ISearchDataProvider, SearchDataProvider>();
            services.AddScoped<ISystemPageDataProvider, SystemPageDataProvider>();

            // Write
            services.AddScoped<ICommentCommand, CommentCommand>();
            services.AddScoped<IDiscussionCommand, DiscussionCommand>();
            services.AddScoped<IGroupCommand, GroupCommand>();
            services.AddScoped<IFileCommand, FileCommand>();
            services.AddScoped<IFolderCommand, FolderCommand>();
            services.AddScoped<IImageCommand, ImageCommand>();
            services.AddScoped<IRolesCommand, RolesCommand>();

            return services;
        }
    }
}
