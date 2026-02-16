using Domain;
using Infrastructure.Execution;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Workers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<JobDbContext>();

            var job = await db.Jobs
                .Where(j => j.Status == JobStatus.Pending)
                .OrderBy(j => j.CreatedAt)
                .FirstOrDefaultAsync(stoppingToken);

            if (job is null)
            {
                await Task.Delay(2000, stoppingToken);
                continue;
            }

            job.Status = JobStatus.Cloning;
            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Cloning repo: {Repo}", job.RepositoryUrl);

            var workspace = Path.GetFullPath(Path.Combine("..","workspaces", job.Id.ToString()));
            Directory.CreateDirectory(workspace);

            var (code, logs) = await ProcessRunner.RunAsync(
                "git",
                $"clone --depth 1 --single-branch --no-tags {job.RepositoryUrl} .",
                workspace,
                stoppingToken
            );

            job.Logs = logs;
            job.Status = code == 0 ? JobStatus.Completed : JobStatus.Failed;

            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Finished job: {Id} (exit {Code})", job.Id, code);
        }
    }
}