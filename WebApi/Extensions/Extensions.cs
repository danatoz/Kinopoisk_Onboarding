using System.Reflection;
using WebApi.Services;
using DbInitialize = WebApi.Services.DbInitialize;

namespace System;

public static class Extensions
{
    public static async Task InitializeDatabase(this WebApplication app)
    {
        var configuration = app.Configuration;

        await using var service_scope = app.Services.CreateAsyncScope();

        var services = service_scope.ServiceProvider;

        var db_initializer = services.GetRequiredService<DbInitialize>();

        await db_initializer.InitializeAsync(
            removeBefore: configuration.GetValue("DbRemoveBefore", false),
            initializeData: configuration.GetValue("DbInitializeData", false));
    }

    public static async Task InitializeCache(this WebApplication app)
    {
        await using var service_scope = app.Services.CreateAsyncScope();

        var services = service_scope.ServiceProvider;

        var cacheInitializer = services.GetRequiredService<CacheInitializeService>();
        await cacheInitializer.CacheInitialize();
    }

    public static T? GetAttribute<T>(this Enum value) 
        where T : Attribute
    {
        return value.GetType().GetMember(value.ToString()).First().GetCustomAttribute<T>();
    }
}