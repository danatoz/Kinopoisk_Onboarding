using System.IO.IsolatedStorage;
using System.Reflection;
using System.Xml.Linq;
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

services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("Default"));
});

services.AddRouting(options => options.LowercaseUrls = true);
//services.AddHttpsRedirection(options =>
//{
//    options.HttpsPort = 5001;
//});

#endregion

var app = builder.Build();

#region Middleware

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API - V1");
});
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSwagger();
});

app.MapControllers();

#endregion

app.Run();
