using System.Reflection;
using Common;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Dal;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


#region Services

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "My API - V1",
                Version = "v1"
            }
        );
        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    }
);

var config = new SharedConfiguration
{
    ApiKey = configuration["ApiKey"]
};
services.AddSingleton(config);

services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("Default"));
});

services.AddRouting(options => options.LowercaseUrls = true);

#endregion


var app = builder.Build();

#region Middleware

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API - V1");
});
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSwagger();
});

app.MapControllers();

#endregion

app.Run();
