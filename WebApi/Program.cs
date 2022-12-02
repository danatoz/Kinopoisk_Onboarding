using Common;
using Microsoft.EntityFrameworkCore;
using Dal;
using WebApi.Services;
using BL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebApi.Authentication;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


#region Services

services.AddControllers();
services.AddEndpointsApiExplorer();
services.SwaggerConfiguration();

var config = new SharedConfiguration
{
    ApiKey = configuration["ApiKey"]
};
services.AddSingleton(config);

services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});
services.AddScoped<DbInitialize>();
services.AddScoped<CacheInitializeService>();
services.AddScoped<MovieBL>();

services.AddRouting(options => options.LowercaseUrls = true);

services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("RedisCacheUrl");
    options.InstanceName = "SampleInstance";
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });
#endregion


var app = builder.Build();

#region Middleware

await app.InitializeDatabase();
await app.InitializeCache();

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
