using Application.Abstractions;
using Domain;

namespace Workers;

public class Worker:BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IJobQueue _queue;

    public Worker(ILogger<Worker> logger, IJobQueue queue)
    {
        _logger = logger;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _queue.DequeueAsync(stoppingToken);

            job.Status = JobStatus.Cloning;
            
            _logger.LogInformation("Processing repo: {Repo}", job.RepositoryUrl);
            
            await Task.Delay(2000, stoppingToken);
            
            job.Status = JobStatus.Completed;
            
            _logger.LogInformation("Finished job: {Id}", job.Id);
        }
    }
}