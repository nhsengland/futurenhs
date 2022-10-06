using FutureNHS.Api.DataAccess.ContentApi.Handlers;
using FutureNHS.Api.DataAccess.ContentApi.Handlers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProvider;
using FutureNHS.Api.DataAccess.Repositories.ContentApi.ContentApiProviders.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;

namespace FutureNHS.Api.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection DataAccess(this IServiceCollection services)
        {
            // RequestHandler
            services.AddScoped<IContentApiRequestHandler, ContentApiRequestHandler>();

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
            services.AddScoped<IContentApiClientProvider, ContentApiClientProvider>();
            services.AddScoped<IUserAdminDataProvider, UserDataProvider>();
            services.AddScoped<IUserDataProvider, UserDataProvider>();
            services.AddScoped<ILikeDataProvider, LikeDataProvider>();
            services.AddScoped<IRegistrationDataProvider, RegistrationDataProvider>();

            // Write
            services.AddScoped<ICommentCommand, CommentCommand>();
            services.AddScoped<IDiscussionCommand, DiscussionCommand>();
            services.AddScoped<IEntityCommand, EntityCommand>();
            services.AddScoped<IGroupCommand, GroupCommand>();
            services.AddScoped<IFileCommand, FileCommand>();
            services.AddScoped<IFolderCommand, FolderCommand>();
            services.AddScoped<IImageCommand, ImageCommand>();
            services.AddScoped<ILikeCommand, LikeCommand>();
            services.AddScoped<IContentCommand, ContentCommand>();
            services.AddScoped<IRolesCommand, RolesCommand>();
            services.AddScoped<IUserCommand, UserCommand>();



            return services;
        }
    }
}
