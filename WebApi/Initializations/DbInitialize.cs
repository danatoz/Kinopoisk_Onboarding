﻿using System.Diagnostics;
using BL;
using Dal.Concrete.Context;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Initializations;

public class DbInitialize
{
    private readonly AppDbContext _dbContext;
    
    private readonly ILogger<DbInitialize> _logger;

    private readonly MovieBL _movieBl;

    private readonly FilterBL _filterBl;

    public DbInitialize(AppDbContext dbContext, ILogger<DbInitialize> logger, MovieBL movieBl, FilterBL filterBl)
    {
        _dbContext = dbContext;
        _logger = logger;
        _movieBl = movieBl;
        _filterBl = filterBl;
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
            _logger.LogInformation("The following migrations have been applied to the database: {0}", string.Join(",", appliedMigrations));

        if (pendingMigrations.Any())
        {
            _logger.LogInformation("Applying migrations: {0}...", string.Join(",", pendingMigrations));
            await _dbContext.Database.MigrateAsync(cancel);
            _logger.LogInformation("Migrations applied");
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

        await _filterBl.Update();

        _logger.LogInformation("Database initialization is successful {0} ms", timer.ElapsedMilliseconds);
    }
}