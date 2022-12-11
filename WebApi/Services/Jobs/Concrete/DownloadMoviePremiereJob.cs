using BL.Abstract;
using Dal.Concrete.Context;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace WebApi.Services.Jobs.Concrete;

public class DownloadMoviePremiereJob : IJob
{
    private readonly ILogger<DownloadMoviePremiereJob> _logger;

    private readonly IMovieService _movieService;

    private readonly AppDbContext _dbContext;

    public DownloadMoviePremiereJob(ILogger<DownloadMoviePremiereJob> logger, IMovieService movieService, AppDbContext dbContext)
    {
        _logger = logger;
        _movieService = movieService;
        _dbContext = dbContext;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Movies\", \"CountryMovie\" RESTART IDENTITY;");
        _logger.LogInformation($"Table Movies, CountryMovie cleared.");

        var result = await _movieService.Download(CancellationToken.None);

        _logger.LogInformation($"Download: {result} premiers.");
    }
}