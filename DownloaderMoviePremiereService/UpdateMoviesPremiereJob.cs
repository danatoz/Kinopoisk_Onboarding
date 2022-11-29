using Quartz;

namespace DownloaderMoviePremiereService;

class UpdateMoviesPremiereJob : IJob
{
    private readonly ILogger<UpdateMoviesPremiereJob> _logger;
    public UpdateMoviesPremiereJob(ILogger<UpdateMoviesPremiereJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogWarning("Job update run!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        
        return Task.CompletedTask;
    }

    private Task Update()
    {

        return Task.CompletedTask;;
    }
}