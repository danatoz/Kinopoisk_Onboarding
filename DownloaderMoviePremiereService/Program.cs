using Common;
using Dal;
using DownloaderMoviePremiereService;
using Quartz;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services ) =>
    {
        var config = new SharedConfiguration
        {
            ApiKey = hostContext.Configuration["ApiKey"]
        };
        services.AddSingleton(config);
        services.AddQuartz(q =>
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(hostContext.Configuration.GetConnectionString("Default"));
            });
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("UpdateMoviesPremiereJob");

            q.AddJob<UpdateMoviesPremiereJob>(options => options.WithIdentity(jobKey));

            q.AddTrigger(options => options
                .ForJob(jobKey)
                .WithIdentity("UpdateMoviesPremiereJob-trigger")
                .WithCronSchedule("10/1 * * * * ?"));//"0 0 0 1 1/1 ? *"
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .Build();

await host.RunAsync();
