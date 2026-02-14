using Application.Abstractions;
using Domain;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class SqliteJobRepository : IJobRepository
{
    private readonly JobDbContext _db;

    public SqliteJobRepository(JobDbContext db)
    {
        _db = db;
    }

    public async Task<ReproductionJob> AddAsync(ReproductionJob job, CancellationToken ct = default)
    {
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync(ct);
        return job;
    }
}