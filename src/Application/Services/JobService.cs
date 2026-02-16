using Application.Abstractions;
using Application.Contracts;
using Domain;

namespace Application.Services;

public class JobService
{
    private readonly IJobRepository _repo;

    public JobService(IJobRepository repo)
    {
        _repo = repo;
    }

    public async Task<ReproductionJob> CreateAsync(CreateJobRequest request)
    {
        var job = new ReproductionJob
        {
            RepositoryUrl = request.RepositoryUrl,
            Status = JobStatus.Pending
        };

        return await _repo.AddAsync(job);
    }

    public Task<ReproductionJob?> GetAsync(Guid id)
    {
        return _repo.GetAsync(id);
    }
}