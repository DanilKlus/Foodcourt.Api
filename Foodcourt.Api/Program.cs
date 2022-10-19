using Foodcourt.Api.DI;
using Microsoft.EntityFrameworkCore;
using Foodcourt.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddRouting(o => o.LowercaseUrls = true)
    .AddControllers()
    .ConfigureJson();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument();

var app = builder.Build();
app
    .UsePathBase("/api")
    .UseRouting()
    .UseEndpoints(endpoints => endpoints.MapControllers());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUi3();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
