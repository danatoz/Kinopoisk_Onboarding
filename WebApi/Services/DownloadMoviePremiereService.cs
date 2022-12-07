using BL;
using Dal.Concrete.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace WebApi.Services;

public class DownloadMoviePremiereService : IHostedService, IDisposable
{
    private int executionCount = 0;
    
    private readonly ILogger<DownloadMoviePremiereService> _logger;

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly AppDbContext? _dbContext;

    private readonly MovieBL _movieBl;

    public DownloadMoviePremiereService(ILogger<DownloadMoviePremiereService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        using var scope = scopeFactory.CreateScope();
        _dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        _movieBl = scope.ServiceProvider.GetRequiredService<MovieBL>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");
        UpdatePremieresScheduledService(cancellationToken);

        return Task.CompletedTask;
    }

    private void UpdatePremieresScheduledService(object task)
    {
        var count = Interlocked.Increment(ref executionCount);

        //await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Movies\", \"CountryMovie\" RESTART IDENTITY;");
        //_logger.LogInformation($"Table Movies, CountryMovie cleared.");
        //
        //var result = await _movieBl.Update(CancellationToken.None);
        //
        //_logger.LogInformation($"Download: {result} premiers.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {

    }
}