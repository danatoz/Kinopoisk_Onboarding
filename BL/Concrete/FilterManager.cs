using BL.Abstract;
using BL.Constants;
using Core.Configurations;
using Core.Entities.Concrete;
using Core.Entities.Models;
using Core.Utilities.Results;
using Dal.Concrete.Context;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BL.Concrete;

public class FilterManager : IFilterService
{
    private readonly AppDbContext _dbContext;

    private readonly SharedConfiguration _sharedConfiguration;

    private readonly ILogger<FilterManager> _logger;

    private readonly UriConstant _uriConstant;

    private int _count = 0;

    private readonly IDistributedCache _cache;

    private readonly IServiceProvider _serviceProvider;

    public FilterManager(AppDbContext dbContext, SharedConfiguration sharedConfiguration, ILogger<FilterManager> logger, UriConstant uriConstant, IDistributedCache cache, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _sharedConfiguration = sharedConfiguration;
        _logger = logger;
        _uriConstant = uriConstant;
        _cache = cache;
        _serviceProvider = serviceProvider;
    }

    public FilterWrapper GetAll()
    {
        var countriesBytes = _cache.Get(RedisKeyConstant.Countries);

        if (countriesBytes == null) return new FilterWrapper();

        var countriesString = Encoding.UTF8.GetString(countriesBytes);
        var countries = JsonConvert.DeserializeObject<List<Country>>(countriesString)
            ?.Select(item => new CountryModel { Id = item.Id, country = item.Name }).ToList();

        var genres = Enum.GetValues(typeof(Core.Entities.Enums.Genre)).Cast<Core.Entities.Enums.Genre>().Select(item => 
            new GenreModel { Id = (int)item, genre = item.GetAttribute<DisplayAttribute>()?.Name }).ToList();

        var result = new FilterWrapper { Genres= genres, Countries = countries };

        return result;
    }

    public async Task<int> Download(CancellationToken cancel)
    {
        try
        {
            var apiService = _serviceProvider.GetService(typeof(IApiService<ResponseFiltersModel>)) as IApiService<ResponseFiltersModel>;
            var wrapperFilterModel = await apiService.Request(new HttpRequestConfiguration
            {
                Uri = _uriConstant.FiltersUri,
                Method = HttpMethod.Get,
                Headers = new Dictionary<string, string>() { { "Accept", "application/json" }, { "X-API-KEY", _sharedConfiguration.ApiKey } }
            });

            if (wrapperFilterModel.StatusCode != HttpStatusCode.OK)
            {
                _count++;
                if (_count == 5)
                    return 0;
                await Task.Delay(new TimeSpan(0, 0, 1), cancel);
                return await Download(cancel);
            }

            var countries = CountryModel.ConvertToEntities(wrapperFilterModel.Data.countries.ToList());

            await _dbContext.Countries.AddRangeAsync(countries, cancel);
            await _dbContext.SaveChangesAsync(cancel);

            return countries.Count;
        }
        catch (Exception e)
        {
            _logger.LogError($"Api is not responding");
            _logger.LogError($"{e.Message}");
        }

        return 0;
    }
}