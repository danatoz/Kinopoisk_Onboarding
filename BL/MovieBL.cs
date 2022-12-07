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

    public MovieBL(AppDbContext dbContext, ILogger<MovieBL> logger, SharedConfiguration sharedConfiguration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sharedConfiguration = sharedConfiguration;
    }

    public async Task<int> Update(CancellationToken cancel)
    {
        try
        {
            var request = new ApiRequestBL<ResponseMoviesModel>();
            var moviesModel = await request.Request(new HttpRequestConfiguration
            {
                Uri = Constant.PremieresUri,
                Method = HttpMethod.Get,
                Headers = new Dictionary<string, string>() { { "Accept", "application/json" }, { "X-API-KEY", _sharedConfiguration.ApiKey } }
            });
            var movies = MovieModel.ConvertToEntities(moviesModel?.items?.ToList());

            await _dbContext.Movies.AddRangeAsync(movies!, cancel);
            await _dbContext.SaveChangesAsync(cancel);

            return movies.Count;
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}");
            _logger.LogError($"{e.StackTrace}");
        }

        await Task.Delay(new TimeSpan(0, 1, 0), cancel);

        return await Update(cancel);
    }
}
