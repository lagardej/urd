using System.Collections.Concurrent;
using Urd.Engine.Clock;
using Urd.Engine.Component;
using Urd.Engine.Messaging;

namespace Urd.Engine.Scheduling;

/// <summary>
///     Discrete-event scheduler. Maintains a min-heap of jobs keyed by next-due simulated time.
///     Incoming <see cref="JobScheduled" /> messages are queued concurrently and drained into the heap
///     at the start of each <see cref="ClockTicked" />. Due jobs are dispatched in parallel.
///     Publishes <see cref="JobStarted" /> immediately before each job is dispatched.
/// </summary>
public sealed class Scheduler
{
    private readonly PriorityQueue<JobScheduled, ulong> _heap = new();
    private readonly ConcurrentQueue<JobScheduled> _intake = new();
    private readonly IMessageBus _bus;

    public Scheduler(IMessageBus bus)
    {
        _bus = bus;
        bus.Subscribe<JobScheduled>(OnJobScheduled);
        bus.Subscribe<ClockTicked>(OnClockTicked);
    }

    public int PendingJobCount => _heap.Count + _intake.Count;

    public IReadOnlyList<JobScheduled> IntakeQueue => _intake.ToList();

    public IReadOnlyList<(JobScheduled Job, ulong DueAt)> HeapSnapshot =>
        _heap.UnorderedItems.Select(item => (item.Element, item.Priority)).ToList();

    public IReadOnlyList<JobScheduled> JobsDueAt(ulong time)
    {
        return _heap.UnorderedItems
            .Where(item => item.Priority <= time)
            .Select(item => item.Element)
            .Concat(_intake.Where(item => item.DueAt <= time))
            .ToList();
    }

    private void OnJobScheduled(JobScheduled evt)
    {
        _intake.Enqueue(evt);
    }

    private void OnClockTicked(ClockTicked evt)
    {
        while (_intake.TryDequeue(out var item))
            _heap.Enqueue(item, item.DueAt);

        var due = new List<JobScheduled>();

        while (_heap.TryPeek(out _, out var dueAt) && dueAt <= evt.ElapsedSeconds)
            due.Add(_heap.Dequeue());

        Parallel.ForEach(due, job =>
        {
            _bus.Publish(new JobStarted(job.ComponentId, evt.ElapsedSeconds));
            job.Tick(new TickContext(evt.ElapsedSeconds, job.CellSet));
        });
    }
}
