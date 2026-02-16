using System.Dynamic;

namespace Domain;

public enum JobStatus
{
    Pending,
    Cloning,
    Building,
    Testing,
    Completed,
    Failed
}

public class ReproductionJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RepositoryUrl { get; set; } = "";
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string Logs { get; set; } = "";
    public DateTime CreatedAt { get; set;} = DateTime.UtcNow;
    public string StatusText => Status.ToString();
}