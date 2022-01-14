using FutureNHS.Api.DataAccess.Repositories.Read;
using FutureNHS.Api.DataAccess.Repositories.Read.Interfaces;

namespace FutureNHS.Api.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection DataAccess(this IServiceCollection services)
        {
            services.AddScoped<IGroupDataProvider, GroupDataProvider>();
            services.AddScoped<IImageDataProvider, ImageDataProvider>();
            services.AddScoped<IHealthCheckDataProvider, HealthCheckDataProvider>();
            services.AddScoped<IRolesDataProvider, RolesDataProvider>();
            services.AddScoped<IPermissionsDataProvider, PermissionsDataProvider>();
            services.AddScoped<IFileAndFolderDataProvider, FileAndFolderDataProvider>();
            return services;
        }
    }
}
