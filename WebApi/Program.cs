using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BL;
using BL.Abstract;
using BL.Concrete;
using BL.Constants;
using BL.DependencyResolvers.Autofac;
using Core.Configurations;
using Core.Extensions;
using Dal.Concrete.Context;
using WebApi.Extensions;
using WebAPI.Extensions;
using WebApi.Initializations;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacBusinessModule()));

#region Services

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwagger();

var config = new SharedConfiguration
{
    ApiKey = configuration["ApiKey"]
};
var uriConstSection = configuration.GetSection("UriConstSection");
var uriConstant = new UriConstant(uriConstSection["FiltersUri"], uriConstSection["PremieresUriWithoutParams"]);
services.AddSingleton(config);
services.AddSingleton(uriConstant);

services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});

services.AddScoped<DbInitialize>();
services.AddScoped<CacheInitializeService>();

services.AddRouting(options => options.LowercaseUrls = true);

services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("RedisCacheUrl");
    options.InstanceName = "SampleInstance";
});

services.AddJwtAuthentication(builder.Configuration);
services.JobsInitialize();
services.AddHostedService<DownloadMoviePremiereService>();
#endregion


var app = builder.Build();

#region Middleware

await app.InitializeDatabase();
await app.InitializeCache();
app.ConfigureCustomExceptionMiddleware();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API - V1");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapSwagger();
});

app.MapControllers();

#endregion

app.Run();
