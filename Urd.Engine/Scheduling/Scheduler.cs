using System.Collections.Concurrent;
using Urd.Engine.Clock;
using Urd.Engine.Messaging;

namespace Urd.Engine.Scheduling;

/// <summary>
/// Discrete-event scheduler. Maintains a min-heap of jobs keyed by next-due simulated time.
/// Incoming <see cref="JobScheduled"/> messages are queued concurrently and drained into the heap
/// at the start of each <see cref="ClockTicked"/>. Due jobs are dispatched in parallel.
/// </summary>
public sealed class Scheduler
{
    private readonly ConcurrentQueue<(Action<ulong> Tick, ulong DueAt)> _intake = new();
    private readonly PriorityQueue<Action<ulong>, ulong> _heap = new();

    public Scheduler(IMessageBus bus)
    {
        bus.Subscribe<JobScheduled>(OnJobScheduled);
        bus.Subscribe<ClockTicked>(OnClockTicked);
    }

    private void OnJobScheduled(JobScheduled evt) => _intake.Enqueue((evt.Tick, evt.DueAt));

    private void OnClockTicked(ClockTicked evt)
    {
        while (_intake.TryDequeue(out var item))
            _heap.Enqueue(item.Tick, item.DueAt);

        var due = new List<Action<ulong>>();

        while (_heap.TryPeek(out _, out var dueAt) && dueAt <= evt.ElapsedSeconds)
            due.Add(_heap.Dequeue());

        Parallel.ForEach(due, action => action(evt.ElapsedSeconds));
    }
}