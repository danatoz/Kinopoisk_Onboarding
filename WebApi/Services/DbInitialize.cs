using System.Diagnostics;
using System.Text;
using BL;
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
    private readonly MovieBL _movieBl;

    public DbInitialize(AppDbContext dbContext, ILogger<DbInitialize> logger, SharedConfiguration configuration, MovieBL movieBl)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sharedConfiguration = configuration;
        _movieBl = movieBl;
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

        await _movieBl.Update(cancel);

        await DownloadFilters(cancel);

        _logger.LogInformation("Инициализация БД данными выполнена успешно за {0} мс", timer.ElapsedMilliseconds);
    }

    private HttpRequestMessage HttpRequestMessage(StringBuilder uri)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("X-API-KEY", _sharedConfiguration.ApiKey);
        return request;
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
            var countries = CountryModel.ConvertToEntities(jsonResult?.countries.ToList());

            if (countries != null)
            {
                await _dbContext.Countries.AddRangeAsync(countries!, cancel);

                await _dbContext.SaveChangesAsync(cancel);
            }
        }
    }
}