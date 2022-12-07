using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using WebApi.Services.Jobs.Concrete;
using WebApi.Services.Jobs;

namespace WebApi.Initializations;

public static class JobInitialize
{
    public static IServiceCollection JobsInitialize(this IServiceCollection services)
    {
        // Add Quartz services
        services.AddSingleton<IJobFactory, SingletonJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

        // Add our job
        services.AddSingleton<DownloadMoviePremiereJob>();
        services.AddSingleton(new JobSchedule(
            jobType: typeof(DownloadMoviePremiereJob),
            cronExpression: "0 0 0 1 1/1 ? *")); // launch every first day of the month "0 0 0 1 1/1 ? *"

        return services;
    }
}