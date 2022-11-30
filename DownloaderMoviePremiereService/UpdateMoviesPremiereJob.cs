using System.Diagnostics.CodeAnalysis;
using Common;
using Common.Enums;
using Quartz;
using System.Text;
using Dal;
using DownloaderMoviePremiereService.Models;
using Entities;
using Newtonsoft.Json;

namespace DownloaderMoviePremiereService;

class UpdateMoviesPremiereJob : IJob
{
    private readonly ILogger<UpdateMoviesPremiereJob> _logger;
    private readonly SharedConfiguration _sharedConfiguration;
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly AppDbContext _dbContext;

    public UpdateMoviesPremiereJob(ILogger<UpdateMoviesPremiereJob> logger, SharedConfiguration sharedConfiguration, AppDbContext dbContext)
    {
        _logger = logger;
        _sharedConfiguration = sharedConfiguration;
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var result = await Update();
        _logger.LogInformation($"Загружено: {result} премьер.");
    }

    private async Task<int> Update()
    {
        var request = HttpRequestMessage();

        using var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonResult = JsonConvert.DeserializeObject<ResponseMoviesModel>(content);
            var movies = MovieModel.ConvertToEntities(jsonResult?.items?.ToList());

            try
            {
                var logEntity = new MoviePremiereUpdateLog { CreationDate = DateTime.UtcNow };
                await _dbContext.MoviePremiereUpdateLogs.AddAsync(logEntity);
                await _dbContext.SaveChangesAsync();

                foreach (var movie in movies.Where(movie => movie != null))
                {
                    movie.MoviePremiereUpdateLogId = logEntity.Id;
                }

                await _dbContext.Movies.AddRangeAsync(movies);
                await _dbContext.SaveChangesAsync();

                return movies.Count;
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}");
                _logger.LogError($"{e.StackTrace}");
            }
        }

        //TODO Что если статус код отрицательный
        Task.Delay(new TimeSpan(0, 1, 0));

        return await Update();
    }

    private HttpRequestMessage HttpRequestMessage()
    {
        var currentMonth = (Months)DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var requestUri = new StringBuilder();

        requestUri.Append("https://kinopoiskapiunofficial.tech/api/v2.2/films/premieres?");
        requestUri.Append($"year={currentYear}");
        requestUri.Append($"&month={currentMonth}");

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri.ToString());
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("X-API-KEY", _sharedConfiguration.ApiKey);
        return request;
    }
}