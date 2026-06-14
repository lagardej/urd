using System.Diagnostics;
using Urd.Engine.Messaging;
using Urd.Engine.Scheduling;

namespace Urd.Engine.Component;

/// <summary>
///     Base class for all components. Handles self-slotting and publishes <see cref="ComponentTicked" />
///     with wall-clock duration after each tick.
/// </summary>
public abstract class BaseComponent
{
    private readonly IMessageBus _bus;

    protected BaseComponent(IMessageBus bus)
    {
        _bus = bus;
    }

    protected abstract ulong Cadence { get; }

    protected void ScheduleAt(ulong dueAt)
    {
        _bus.Publish(new JobScheduled(dueAt, OnTick));
    }

    private void OnTick(ulong elapsedSeconds)
    {
        var sw = Stopwatch.StartNew();
        Tick(elapsedSeconds);
        sw.Stop();
        _bus.Publish(new ComponentTicked(GetType().Name, elapsedSeconds, sw.Elapsed));
        ScheduleAt(elapsedSeconds + Cadence);
    }

    protected abstract void Tick(ulong elapsedSeconds);
}