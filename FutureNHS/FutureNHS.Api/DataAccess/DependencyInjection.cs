﻿using FutureNHS.Api.DataAccess.Repositories.Read;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;
using FutureNHS.Api.DataAccess.Repositories.Write;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;

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

            // Write
            services.AddScoped<IGroupCommand, GroupCommand>();
            services.AddScoped<IRolesCommand, RolesCommand>();

            return services;
        }
    }
}
