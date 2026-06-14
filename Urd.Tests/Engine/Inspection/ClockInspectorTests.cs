using Urd.Engine.Clock;
using Urd.Engine.Inspection;
using Xunit;

namespace Urd.Tests.Engine.Inspection;

public sealed class ClockInspectorTests
{
    private readonly InMemoryMessageBus _bus = new();
    private TimeSpan                    _wallClock;
    private readonly ClockInspector     _inspector;

    public ClockInspectorTests()
    {
        _inspector = new ClockInspector(_bus, () => _wallClock);
    }

    [Fact]
    public void ElapsedSeconds_IsZeroBeforeFirstTick()
    {
        Assert.Equal(0ul, _inspector.ElapsedSeconds);
    }

    [Fact]
    public void ElapsedSeconds_ReflectsLastTickValue()
    {
        _bus.Publish(new ClockTicked(3600));

        Assert.Equal(3600ul, _inspector.ElapsedSeconds);
    }

    [Fact]
    public void ElapsedSeconds_UpdatesOnSubsequentTicks()
    {
        _bus.Publish(new ClockTicked(3600));
        _bus.Publish(new ClockTicked(7200));

        Assert.Equal(7200ul, _inspector.ElapsedSeconds);
    }

    [Fact]
    public void CompressionRatio_IsZeroBeforeFirstTick()
    {
        _wallClock = TimeSpan.FromSeconds(1);

        Assert.Equal(0.0, _inspector.CompressionRatio);
    }

    [Fact]
    public void CompressionRatio_IsZeroWhenWallClockIsZero()
    {
        _bus.Publish(new ClockTicked(3600));
        _wallClock = TimeSpan.Zero;

        Assert.Equal(0.0, _inspector.CompressionRatio);
    }

    [Fact]
    public void CompressionRatio_ReflectsSimulatedOverWallClock()
    {
        _bus.Publish(new ClockTicked(3600));
        _wallClock = TimeSpan.FromSeconds(360);

        Assert.Equal(10.0, _inspector.CompressionRatio);
    }

    [Fact]
    public void WallClockElapsed_ReflectsInjectedClock()
    {
        _wallClock = TimeSpan.FromSeconds(42);

        Assert.Equal(TimeSpan.FromSeconds(42), _inspector.WallClockElapsed);
    }
}
