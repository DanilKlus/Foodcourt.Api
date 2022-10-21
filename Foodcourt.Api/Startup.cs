using Foodcourt.Api.DI;
using Foodcourt.Data;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers()
                .ConfigureJson();
            
            services.AddSystemServices();
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUi3();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}







// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// // builder.Services.AddEndpointsApiExplorer();
// // builder.Services.AddSwaggerDocument();








