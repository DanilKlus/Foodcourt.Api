using Foodcourt.BusinessLogic.Services.Auth;
using Foodcourt.BusinessLogic.Services.Basket;
using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.BusinessLogic.Services.Orders;
using Foodcourt.BusinessLogic.Services.Users;

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
                .AddScoped<ICafeService, CafeService>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IUserService, UserService>();
            return services;
        }
        
        private const string FrontSpecificOrigins = "_frontSpecificOrigins";
        public static IServiceCollection AddServiceCors(this IServiceCollection services)
        {
            services.AddCors(
                option =>
                {
                    option.AddPolicy(
                        FrontSpecificOrigins,
                        builder =>
                            builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
                });
            return services;
        }
    }
}