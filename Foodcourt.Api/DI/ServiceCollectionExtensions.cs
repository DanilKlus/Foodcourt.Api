using Foodcourt.BusinessLogic.Services.Auth;
using Foodcourt.BusinessLogic.Services.Cafes;

namespace Foodcourt.Api.DI
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemServices(this IServiceCollection services)
        {
            services
                .AddDistributedMemoryCache()
                .AddScoped<ICafeService, CafeService>()
                .AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}