using Foodcourt.BusinessLogic.Services.Cafes;

namespace Foodcourt.Api.DI
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemServices(this IServiceCollection services)
        {
            services
                .AddDistributedMemoryCache()
                .AddScoped<ICafeRepository, CafeRepository>()
                .AddScoped<ICafeService, CafeService>();
            return services;
        }
    }
}