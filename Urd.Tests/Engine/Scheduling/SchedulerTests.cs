using Urd.Engine.Clock;
using Urd.Engine.Scheduling;
using Xunit;

namespace Urd.Tests.Engine.Scheduling;

public sealed class SchedulerTests
{
    private readonly InMemoryMessageBus _bus = new();

    [Fact]
    public void PendingJobCount_ReflectsEnqueuedJobs()
    {
        var scheduler = new Scheduler(_bus);
        _ = new DummyComponent(_bus);

        Assert.Equal(1, scheduler.PendingJobCount);
    }

    [Fact]
    public void IntakeQueue_ContainsJobBeforeClockTick()
    {
        var scheduler = new Scheduler(_bus);
        _ = new DummyComponent(_bus);

        Assert.Single(scheduler.IntakeQueue);
    }

    [Fact]
    public void HeapSnapshot_ContainsJobAfterClockTick()
    {
        var scheduler = new Scheduler(_bus);
        _ = new DummyComponent(_bus);

        _bus.Publish(new ClockTicked(1));

        Assert.Empty(scheduler.HeapSnapshot);
        Assert.Single(scheduler.IntakeQueue);
    }

    [Fact]
    public void HeapSnapshot_ContainsJobOnceDrained()
    {
        var scheduler = new Scheduler(_bus);
        _ = new DummyComponent(_bus);

        _bus.Publish(new ClockTicked(1));
        _bus.Publish(new ClockTicked(2));

        Assert.Single(scheduler.HeapSnapshot);
        Assert.Equal(11ul, scheduler.HeapSnapshot[0].DueAt);
    }

    [Fact]
    public void JobsDueAt_ReturnsJobsDueAtOrBefore()
    {
        var scheduler = new Scheduler(_bus);
        _ = new DummyComponent(_bus);

        _bus.Publish(new ClockTicked(1));

        var due = scheduler.JobsDueAt(11);
        Assert.Single(due);
    }

    [Fact]
    public void JobsDueAt_ReturnsEmptyWhenNoneDue()
    {
        var scheduler = new Scheduler(_bus);
        _ = new DummyComponent(_bus);

        _bus.Publish(new ClockTicked(1));

        var due = scheduler.JobsDueAt(0);
        Assert.Empty(due);
    }
}