using System.Threading.Channels;
using Application.Abstractions;
using Domain;

namespace Infrastructure.Queues;

public class InMemoryJobQueue : IJobQueue
{
    private readonly Channel<ReproductionJob> _queue = Channel.CreateUnbounded<ReproductionJob>();

    public void Enqueue(ReproductionJob job)
        => _queue.Writer.TryWrite(job);

    public async ValueTask<ReproductionJob> DequeueAsync(CancellationToken token)
        => await _queue.Reader.ReadAsync(token);
}