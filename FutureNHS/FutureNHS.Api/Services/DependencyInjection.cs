using FutureNHS.Api.Services.Interfaces;

namespace FutureNHS.Api.Services
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<IPermissionsService, PermissionsService>();

            return services;
        }
    }
}
