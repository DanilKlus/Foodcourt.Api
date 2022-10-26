using Foodcourt.BusinessLogic.Services.Auth;
using Foodcourt.BusinessLogic.Services.Basket;
using Foodcourt.BusinessLogic.Services.Cafes;

namespace Foodcourt.Api.DI
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemServices(this IServiceCollection services)
        {
            services
                .AddDistributedMemoryCache()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IBasketService, BasketService>()
                .AddScoped<ICafeService, CafeService>();
            return services;
        }
    }
}