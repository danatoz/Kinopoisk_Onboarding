using Quartz;
using System.Text;
using BL;
using DownloaderMoviePremiereService.Models;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Dal.Concrete.Context;

namespace DownloaderMoviePremiereService;

class UpdateMoviesPremiereJob : IJob
{
    private readonly ILogger<UpdateMoviesPremiereJob> _logger;
    private readonly AppDbContext _dbContext;
    private readonly MovieBL _movieBl;

    public UpdateMoviesPremiereJob(ILogger<UpdateMoviesPremiereJob> logger, AppDbContext dbContext, MovieBL movieBl)
    {
        _logger = logger;
        _dbContext = dbContext;
        _movieBl = movieBl;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Movies\", \"CountryMovie\" RESTART IDENTITY;");
        _logger.LogInformation($"Таблицы Movies, CountryMovie очищены.");

        var result = await _movieBl.Update(CancellationToken.None);
        
        _logger.LogInformation($"Загружено: {result} премьер.");
    }
}