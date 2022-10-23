using Foodcourt.BusinessLogic.Services.Cafes;
using Foodcourt.BusinessLogic.Services.Users;

namespace Foodcourt.Api.DI
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemServices(this IServiceCollection services)
        {
            services
                .AddDistributedMemoryCache()
                .AddScoped<ICafeService, CafeService>()
                .AddScoped<IUserService, UserService>();
            return services;
        }
    }
}