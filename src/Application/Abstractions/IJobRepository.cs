using Domain;

namespace Application.Abstractions;

public interface IJobRepository
{
    Task<ReproductionJob> AddAsync(ReproductionJob job, CancellationToken ct = default);
    Task<ReproductionJob?> GetAsync(Guid id, CancellationToken ct = default);
}