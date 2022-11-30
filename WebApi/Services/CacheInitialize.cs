using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Dal;
using Microsoft.Extensions.Caching.Distributed;

namespace WebApi.Services;

public class CacheInitializeService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _dbContext;
    public CacheInitializeService(IDistributedCache cache, AppDbContext dbContext)
    {
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task CacheInitialize()
    {
        var countries = _dbContext.Countries.ToList();
        var chacheDataString = JsonConvert.SerializeObject(countries);
        var dataToCache = Encoding.UTF8.GetBytes(chacheDataString);
        await _cache.SetAsync("countries", dataToCache);
    }
}