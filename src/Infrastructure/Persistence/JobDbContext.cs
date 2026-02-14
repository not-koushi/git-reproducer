using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class JobDbContext : DbContext
{
    public DbSet<ReproductionJob> Jobs => Set<ReproductionJob>();

    public JobDbContext(DbContextOptions<JobDbContext> options)
        : base(options) { }
}