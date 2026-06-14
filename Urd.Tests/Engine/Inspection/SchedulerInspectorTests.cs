using Urd.Engine.Clock;
using Urd.Engine.Component;
using Urd.Engine.Inspection;
using Urd.Engine.Scheduling;
using Xunit;

namespace Urd.Tests.Engine.Inspection;

public sealed class SchedulerInspectorTests
{
    private readonly InMemoryMessageBus _bus = new();
    private readonly SchedulerInspector _inspector;

    private static JobStarted       AnyJob       => new("c", 1);
    private static ComponentTicked  AnyComponent => new("c", 1, TimeSpan.Zero);
    private static ClockTicked      AnyTick      => new(1);

    public SchedulerInspectorTests()
    {
        // SchedulerInspector must subscribe before Scheduler so ClockTicked resets
        // _dispatchedThisTick/_peakThisTick before dispatch fires (registration order preserved).
        _inspector = new SchedulerInspector(_bus);
    }

    [Fact]
    public void InitialState_IsZero()
    {
        Assert.Equal(0, _inspector.JobsDispatchedLastTick);
        Assert.Equal(0, _inspector.PeakConcurrencyLastTick);
        Assert.Equal(0L, _inspector.TotalJobsDispatched);
    }

    [Fact]
    public void JobsDispatchedLastTick_ReflectsJobsFromPreviousTick()
    {
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyTick);

        Assert.Equal(2, _inspector.JobsDispatchedLastTick);
    }

    [Fact]
    public void TotalJobsDispatched_AccumulatesAcrossTicks()
    {
        _bus.Publish(AnyJob);
        _bus.Publish(AnyTick);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyTick);

        Assert.Equal(3L, _inspector.TotalJobsDispatched);
    }

    [Fact]
    public void JobsDispatchedLastTick_ResetsEachTick()
    {
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyTick);

        _bus.Publish(AnyJob);
        _bus.Publish(AnyTick);

        Assert.Equal(1, _inspector.JobsDispatchedLastTick);
    }

    [Fact]
    public void PeakConcurrencyLastTick_ReflectsMaxOverlap()
    {
        // Start 3 jobs without completing any → peak = 3
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyTick);

        Assert.Equal(3, _inspector.PeakConcurrencyLastTick);
    }

    [Fact]
    public void PeakConcurrencyLastTick_AccountsForCompletedJobs()
    {
        // Start 2, complete 1, start 1 more → max active at once = 2
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyComponent); // active: 1
        _bus.Publish(AnyJob);       // active: 2
        _bus.Publish(AnyTick);

        Assert.Equal(2, _inspector.PeakConcurrencyLastTick);
    }

    [Fact]
    public void PeakConcurrencyLastTick_ResetsEachTick()
    {
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyJob);
        _bus.Publish(AnyComponent);
        _bus.Publish(AnyComponent);
        _bus.Publish(AnyComponent);
        _bus.Publish(AnyTick);

        _bus.Publish(AnyJob);
        _bus.Publish(AnyComponent);
        _bus.Publish(AnyTick);

        Assert.Equal(1, _inspector.PeakConcurrencyLastTick);
    }

    [Fact]
    public void NoJobsInTick_YieldsZeroDispatchedAndPeak()
    {
        _bus.Publish(AnyTick);

        Assert.Equal(0, _inspector.JobsDispatchedLastTick);
        Assert.Equal(0, _inspector.PeakConcurrencyLastTick);
    }
}
