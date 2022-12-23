using System.Text;
using Foodcourt.Api.DI;
using Foodcourt.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Foodcourt.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private readonly string FrontSpecificOrigins = "_frontSpecificOrigins";
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDataContext>(options =>
            {
                var builder = new NpgsqlDbContextOptionsBuilder(options);
                builder.SetPostgresVersion(new Version(9, 2));
                options.UseNpgsql(Configuration["ConnectionStrings:DefaultConnection"]);
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<AppDataContext>()
                .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider,
                    typeof(AuthenticatorTokenProvider<IdentityUser>))
                .AddTokenProvider(Configuration["AuthSettings:ApiTokenProvider"],
                    typeof(DataProtectorTokenProvider<IdentityUser>))
                .AddTokenProvider(Configuration["GoogleAuthSettings:GoogleTokenProvider"],
                typeof(DataProtectorTokenProvider<IdentityUser>));
            
            services.Configure<IdentityOptions>(options =>
            { 
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            });
            
            services.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["AuthSettings:Audience"],
                        ValidIssuer = Configuration["AuthSettings:Issuer"],
                        RequireExpirationTime = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                        ValidateIssuerSigningKey = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                context.Response.Headers.Add("Token-Expired", "true");
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["GoogleAuthSettings:ClientId"];
                    options.ClientSecret = Configuration["GoogleAuthSettings:ClientSecret"];
                });

            services.AddControllers()
                .ConfigureJson();
            services.AddSystemServices();
            services.AddServiceCors();
            services.AddSwaggerDocument(doc => doc.Title = "Foodcourt.Api");
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseDeveloperExceptionPage()
                .UseSwagger()
                .UseSwaggerUi3()
                .UseOpenApi();
            

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(FrontSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}