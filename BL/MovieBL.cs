using System.Text;
using BL.Models;
using Common;
using Common.Enums;
using Dal;
using Entities;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace BL;

public class MovieBL
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient = new HttpClient();
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
        var requestUri = BuildRequestForPremieres();
        var request = HttpRequestMessage(requestUri);

        using var response = await _httpClient.SendAsync(request, cancel);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancel);
            var jsonResult = JsonConvert.DeserializeObject<ResponseMoviesModel>(content);
            var movies = MovieModel.ConvertToEntities(jsonResult?.items?.ToList());

            try
            {
                var logEntity = new MoviePremiereUpdateLog { CreationDate = DateTime.UtcNow };
                await _dbContext.MoviePremiereUpdateLogs.AddAsync(logEntity, cancel);
                await _dbContext.SaveChangesAsync(cancel);

                foreach (var movie in movies.Where(movie => movie != null))
                {
                    if (movie != null) movie.MoviePremiereUpdateLogId = logEntity.Id;
                }

                await _dbContext.Movies.AddRangeAsync(movies!, cancel);
                await _dbContext.SaveChangesAsync(cancel);

                return movies.Count;
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}");
                _logger.LogError($"{e.StackTrace}");
            }
        }

        //TODO Что если статус код отрицательный
        await Task.Delay(new TimeSpan(0, 1, 0), cancel);

        return await Update(cancel);
    }

    private static StringBuilder BuildRequestForPremieres()
    {
        var currentMonth = (Months)DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var requestUri = new StringBuilder();

        requestUri.Append("https://kinopoiskapiunofficial.tech/api/v2.2/films/premieres?");
        requestUri.Append($"year={currentYear}");
        requestUri.Append($"&month={currentMonth}");
        return requestUri;
    }

    private HttpRequestMessage HttpRequestMessage(StringBuilder uri)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("X-API-KEY", _sharedConfiguration.ApiKey);
        return request;
    }
}
