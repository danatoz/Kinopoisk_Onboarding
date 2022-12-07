using System.Net;
using System.Text;
using Core.Configurations;
using Core.Entities.Enums;
using Core.Entities.Models;
using Microsoft.Extensions.Logging;
using Dal.Concrete.Context;
using BL.Constants;

namespace BL;

public class MovieBL
{
    private readonly AppDbContext _dbContext;
    
    private readonly ILogger<MovieBL> _logger;
    
    private readonly SharedConfiguration _sharedConfiguration;

    private readonly UriConstant _uriConstant;

    private int _count = 0;

    public MovieBL(AppDbContext dbContext, ILogger<MovieBL> logger, SharedConfiguration sharedConfiguration, UriConstant uriConstant)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sharedConfiguration = sharedConfiguration;
        _uriConstant = uriConstant;
    }

    public async Task<int> Update(CancellationToken cancel)
    {
        try
        {
            var request = new ApiRequestBL<ResponseMoviesModel>();
            var wrapperMoviesModel = await request.Request(new HttpRequestConfiguration
            {
                Uri = _uriConstant.PremieresUri,
                Method = HttpMethod.Get,
                Headers = new Dictionary<string, string>() { { "Accept", "application/json" }, { "X-API-KEY", _sharedConfiguration.ApiKey } }
            });

            if (wrapperMoviesModel.StatusCode != HttpStatusCode.OK)
            {
                _count++;
                if (_count == 5)
                    return 0;
                await Task.Delay(new TimeSpan(0, 0, 1), cancel);
                return await Update(cancel);
            }

            var movies = MovieModel.ConvertToEntities(wrapperMoviesModel.Data.items?.ToList());

            await _dbContext.Movies.AddRangeAsync(movies!, cancel);
            await _dbContext.SaveChangesAsync(cancel);

            return movies.Count;
        }
        catch (Exception e)
        {
            _logger.LogError($"Api is not responding");
            _logger.LogError($"{e.Message}");
            //_logger.LogError($"{e.StackTrace}");
        }

        return 0;
    }
}
