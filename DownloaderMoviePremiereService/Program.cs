using Dal;
using DownloaderMoviePremiereService;
using Quartz;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostcontext, services ) =>
    {
        
        services.AddQuartz(q =>
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(hostcontext.Configuration.GetConnectionString("Default"));
            });
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("UpdateMoviesPremiereJob");

            q.AddJob<UpdateMoviesPremiereJob>(options => options.WithIdentity(jobKey));

            q.AddTrigger(options => options
                .ForJob(jobKey)
                .WithIdentity("UpdateMoviesPremiereJob-trigger")
                .WithCronSchedule("0 0 0 1 1/1 ? *"));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .Build();

await host.RunAsync();
