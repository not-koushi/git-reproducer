using Domain;

namespace Application.Abstractions;

public interface IJobQueue
{
    void Enqueue(ReproductionJob job);
    ValueTask<ReproductionJob> DequeueAsync(CancellationToken token);
}