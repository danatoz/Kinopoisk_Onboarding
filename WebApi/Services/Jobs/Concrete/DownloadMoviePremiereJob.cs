using BL;
using Dal.Concrete.Context;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace WebApi.Services.Jobs.Concrete;

public class DownloadMoviePremiereJob : IJob
{
    private readonly ILogger<DownloadMoviePremiereJob> _logger;

    private readonly MovieBL _movieBL;

    private readonly AppDbContext _dbContext;

    public DownloadMoviePremiereJob(ILogger<DownloadMoviePremiereJob> logger, MovieBL movieBl, AppDbContext dbContext)
    {
        _logger = logger;
        _movieBL = movieBl;
        _dbContext = dbContext;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Movies\", \"CountryMovie\" RESTART IDENTITY;");
        _logger.LogInformation($"Table Movies, CountryMovie cleared.");

        var result = await _movieBL.Update(CancellationToken.None);

        _logger.LogInformation($"Download: {result} premiers.");
    }
}