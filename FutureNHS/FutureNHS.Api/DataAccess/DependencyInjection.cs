using FutureNHS.Api.DataAccess.Repositories.Read;
using FutureNHS.Infrastructure.Repositories.Read;
using FutureNHS.Infrastructure.Repositories.Read.Interfaces;

namespace FutureNHS.Api.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection DataAccess(this IServiceCollection services)
        {
            //services.AddTransient<IDbRetryPolicy, DbRetryPolicy>();
            //services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<IGroupDataProvider, GroupDataProvider>();
            services.AddScoped<IImageDataProvider, ImageDataProvider>();
            services.AddScoped<IHealthCheckDataProvider, HealthCheckDataProvider>();

            return services;
        }
    }
}
