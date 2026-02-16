using Application.Abstractions;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ReproductionJob?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }
}