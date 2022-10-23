using System.Text;
using Foodcourt.Api.DI;
using Foodcourt.Data;
using Foodcourt.Data.Api.Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

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
            services.AddDbContext<AppDataContext>(options =>
            {
                var builder = new NpgsqlDbContextOptionsBuilder(options);
                builder.SetPostgresVersion(new Version(9, 2));
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDataContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider(Configuration["AuthSettings:ApiTokenProvider"], typeof(DataProtectorTokenProvider<IdentityUser>));
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // .AddCookie()
                .AddJwtBearer(options =>
            {
                // options.RequireHttpsMetadata = true;
                // options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["AuthSettings:Audience"],
                    ValidIssuer = Configuration["AuthSettings:Issuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
                // options.Events = new JwtBearerEvents
                // {
                //     OnAuthenticationFailed = context =>
                //     {
                //         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                //         {
                //             context.Response.Headers.Add("Token-Expired", "true");
                //         }
                //         return Task.CompletedTask;
                //     }
                // };
            })
                // .AddGoogle(options =>
                // {
                //
                //     options.ClientId = "637404875357-rtn0bvhr0bkbikunu24tk12le0o40h2u.apps.googleusercontent.com";
                //     options.ClientSecret = "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";
                // })
                ;
            
            // services.Configure<CookiePolicyOptions>(options =>
            // {
            //     // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //     options.CheckConsentNeeded = context => true;
            //     options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            // });
            
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

            app.UseAuthentication();
            app.UseAuthorization();

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








