using Core.Configurations;
using Core.Entities.Models;
using BL.Constants;
using Dal.Concrete.Context;
using Microsoft.Extensions.Logging;

namespace BL;

public class FilterBL
{
    private readonly AppDbContext _dbContext;

    private readonly SharedConfiguration _sharedConfiguration;

    private readonly ILogger<FilterBL> _logger;

    public FilterBL(AppDbContext dbContext, SharedConfiguration sharedConfiguration, ILogger<FilterBL> logger)
    {
        _dbContext = dbContext;
        _sharedConfiguration = sharedConfiguration;
        _logger = logger;
    }

    public async Task<int> Update()
    {
        try
        {
            var request = new ApiRequestBL<ResponseFiltersModel>();
            var moviesModel = await request.Request(new HttpRequestConfiguration
            {
                Uri = Constant.FiltersUri,
                Method = HttpMethod.Get,
                Headers = new Dictionary<string, string>() { { "Accept", "application/json" }, { "X-API-KEY", _sharedConfiguration.ApiKey } }
            });

            var countries = CountryModel.ConvertToEntities(moviesModel?.countries.ToList());

            await _dbContext.Countries.AddRangeAsync(countries);

            await _dbContext.SaveChangesAsync();
            return countries.Count;
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}");
            _logger.LogError($"{e.StackTrace}");
        }

        return 0;
    }
}