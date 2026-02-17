using System.Diagnostics;
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
            
            if (code != 0)
            {
                job.Status = JobStatus.Failed;
                await db.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Clone failed: {Id}", job.Id);
                continue;
            }

            var hasDotnetProject =
                Directory.GetFiles(workspace, "*.sln", SearchOption.AllDirectories).Any() ||
                Directory.GetFiles(workspace, "*.csproj", SearchOption.AllDirectories).Any();

            if (!hasDotnetProject)
            {
                job.Status = JobStatus.Completed;
                await db.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("No .NET project detected: {Id}", job.Id);
                continue;
            }

            job.Status = JobStatus.Building;
            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Building .NET project: {Id}", job.Id);

            var (restoreCode, restoreLogs) = await ProcessRunner.RunAsync(
                "dotnet",
                "restore",
                workspace,
                stoppingToken
            );

            var (buildCode, buildLogs) = await ProcessRunner.RunAsync(
                "dotnet",
                "build --no-restore",
                workspace,
                stoppingToken
            );

            job.Logs += "\n--- RESTORE ---\n" + restoreLogs;
            job.Logs += "\n--- BUILD ---\n" +buildLogs;

            job.Status = buildCode == 0 ? JobStatus.Completed : JobStatus.Failed;

            await db.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Build finished: {Id} (exit {Code})", job.Id, buildCode);
        }
    }
}