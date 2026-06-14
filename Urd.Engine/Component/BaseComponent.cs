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

    protected void ScheduleAt(ulong dueAt, CellSet? cellSet = null)
    {
        _bus.Publish(new JobScheduled(GetType().Name, dueAt, OnTick, cellSet ?? CellSet.Global));
    }

    private void OnTick(TickContext context)
    {
        var sw = Stopwatch.StartNew();
        Tick(context);
        sw.Stop();
        _bus.Publish(new ComponentTicked(GetType().Name, context.ElapsedSeconds, sw.Elapsed));
        ScheduleAt(context.ElapsedSeconds + Cadence, context.CellSet);
    }

    protected abstract void Tick(TickContext context);
}
