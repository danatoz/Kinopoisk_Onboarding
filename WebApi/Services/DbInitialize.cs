using System.Diagnostics;
using System.Text;
using Common;
using Common.Enums;
using Dal;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using WebApi.Models;

namespace WebApi.Services;

public class DbInitialize
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DbInitialize> _logger;
    private readonly SharedConfiguration _sharedConfiguration;
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly IDistributedCache _cache;

    public DbInitialize(AppDbContext dbContext, ILogger<DbInitialize> logger, SharedConfiguration configuration, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sharedConfiguration = configuration;
        _cache = cache;
    }

    public async Task DeleteAsync(CancellationToken cancel)
    {
        await _dbContext.Database.EnsureDeletedAsync(cancel).ConfigureAwait(false);
    }

    public async Task InitializeAsync(bool removeBefore = false, bool initializeData = false,
        CancellationToken cancel = default)
    {
        if (removeBefore)
            await DeleteAsync(cancel).ConfigureAwait(false);

        var pendingMigrations =
            await _dbContext.Database.GetPendingMigrationsAsync(cancel).ConfigureAwait(false);
        var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync(cancel);

        if (appliedMigrations.Any())
            _logger.LogInformation("К БД применены миграции: {0}", string.Join(",", appliedMigrations));

        if (pendingMigrations.Any())
        {
            _logger.LogInformation("Применение миграций: {0}...", string.Join(",", pendingMigrations));
            await _dbContext.Database.MigrateAsync(cancel);
            _logger.LogInformation("Применение миграций выполнено");
        }
        else
        {
            await _dbContext.Database.EnsureCreatedAsync(cancel);
        }

        if (initializeData)
        {
            await InitializeDataAsync(cancel);
        }
    }

    public async Task InitializeDataAsync(CancellationToken cancel)
    {
        var timer = Stopwatch.StartNew();

        await Update(cancel);

        await DownloadFilters(cancel);

        _logger.LogInformation("Инициализация БД данными выполнена успешно за {0} мс", timer.ElapsedMilliseconds);
    }

    private async Task<int> Update(CancellationToken cancel)
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
                    movie.MoviePremiereUpdateLogId = logEntity.Id;
                }

                await _dbContext.Movies.AddRangeAsync(movies, cancel);
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
        Task.Delay(new TimeSpan(0, 1, 0));

        return await Update(cancel);
    }

    private HttpRequestMessage HttpRequestMessage(StringBuilder uri)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("X-API-KEY", _sharedConfiguration.ApiKey);
        return request;
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

    private async Task DownloadFilters(CancellationToken cancel)
    {
        var uri = new StringBuilder("https://kinopoiskapiunofficial.tech/api/v2.2/films/filters");
        var request = HttpRequestMessage(uri);
        using var response = await _httpClient.SendAsync(request, cancel);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonResult = JsonConvert.DeserializeObject<ResponseFiltersModel>(content);
            var countries = CountryModel.ConvertToEntities(jsonResult.countries.ToList());

            if (countries != null)
            {
                await _dbContext.Countries.AddRangeAsync(countries, cancel);

                await _dbContext.SaveChangesAsync(cancel);
            }
        }
    }
}