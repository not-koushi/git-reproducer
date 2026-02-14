using System.Runtime.CompilerServices;
using Application.Abstractions;
using Application.Contracts;
using Domain;

namespace Application.Services;

public class JobService
{
    private readonly IJobQueue _queue;

    public JobService(IJobQueue queue)
    {
        _queue = queue;
    }

    public ReproductionJob Create(CreateJobRequest request)
    {
        var job = new ReproductionJob
        {
          RepositoryUrl = request.RepositoryUrl,
          Status = JobStatus.Pending  
        };

        _queue.Enqueue(job);
        return job; 
    }
}