using Urd.Engine.Component;
using Urd.Engine.Profiling;
using Xunit;

namespace Urd.Tests.Engine.Profiling;

public sealed class ProfilerTests
{
    private readonly InMemoryMessageBus _bus = new();

    private static ComponentTicked Evt(string id, TimeSpan duration)
    {
        return new ComponentTicked(id, 0, duration);
    }

    // ── Registration ─────────────────────────────────────────────────────────

    [Fact]
    public void Profiles_EmptyBeforeAnyEvent()
    {
        var profiler = new Profiler(_bus);

        Assert.Empty(profiler.Profiles);
    }

    [Fact]
    public void Profiles_CreatesEntryOnFirstEvent()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(10)));

        Assert.True(profiler.Profiles.ContainsKey("A"));
    }

    [Fact]
    public void Profiles_TracksMultipleComponents()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(1)));
        _bus.Publish(Evt("B", TimeSpan.FromMilliseconds(2)));

        Assert.Equal(2, profiler.Profiles.Count);
    }

    // ── Total stats ──────────────────────────────────────────────────────────

    [Fact]
    public void TickCount_IncrementsPerEvent()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(1)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(2)));

        Assert.Equal(2, profiler.Profiles["A"].TickCount);
    }

    [Fact]
    public void LastDuration_ReflectsMostRecentEvent()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(1)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(5)));

        Assert.Equal(TimeSpan.FromMilliseconds(5), profiler.Profiles["A"].LastDuration);
    }

    [Fact]
    public void TotalDuration_AccumulatesAcrossTicks()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(3)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(7)));

        Assert.Equal(TimeSpan.FromMilliseconds(10), profiler.Profiles["A"].TotalDuration);
    }

    [Fact]
    public void MinDuration_TracksSmallestDuration()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(10)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(2)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(6)));

        Assert.Equal(TimeSpan.FromMilliseconds(2), profiler.Profiles["A"].MinDuration);
    }

    [Fact]
    public void MaxDuration_TracksLargestDuration()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(10)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(2)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(6)));

        Assert.Equal(TimeSpan.FromMilliseconds(10), profiler.Profiles["A"].MaxDuration);
    }

    [Fact]
    public void AverageDuration_IsCorrect()
    {
        var profiler = new Profiler(_bus);

        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(4)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(8)));

        Assert.Equal(TimeSpan.FromMilliseconds(6), profiler.Profiles["A"].AverageDuration);
    }

    [Fact]
    public void AverageDuration_IsZeroBeforeAnyEvent()
    {
        var profiler = new Profiler(_bus);
        var profile = new ComponentProfile(Profiler.DefaultWindowSize);

        Assert.Equal(TimeSpan.Zero, profile.AverageDuration);
    }

    // ── Window / percentiles ─────────────────────────────────────────────────

    [Fact]
    public void WindowSize_DefaultIs1000()
    {
        var profiler = new Profiler(_bus);
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(1)));

        Assert.Equal(1000, profiler.Profiles["A"].WindowSize);
    }

    [Fact]
    public void WindowSize_HonoursCustomValue()
    {
        var profiler = new Profiler(_bus, 5);
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(1)));

        Assert.Equal(5, profiler.Profiles["A"].WindowSize);
    }

    [Fact]
    public void Window_DropsOldestTickOnceCapReached()
    {
        var profiler = new Profiler(_bus, 3);

        // Feed 4 ticks; oldest (1 ms) must be evicted so P50 reflects [2,3,4].
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(1)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(2)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(3)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(4)));

        // TotalDuration still counts all 4 ticks.
        Assert.Equal(TimeSpan.FromMilliseconds(10), profiler.Profiles["A"].TotalDuration);
        Assert.Equal(4, profiler.Profiles["A"].TickCount);

        // Window percentiles only see [2, 3, 4].
        Assert.Equal(TimeSpan.FromMilliseconds(3), profiler.Profiles["A"].WindowP50);
    }

    [Fact]
    public void WindowPercentiles_AreZeroBeforeAnyEvent()
    {
        var profile = new ComponentProfile(Profiler.DefaultWindowSize);

        Assert.Equal(TimeSpan.Zero, profile.WindowP50);
        Assert.Equal(TimeSpan.Zero, profile.WindowP95);
        Assert.Equal(TimeSpan.Zero, profile.WindowP99);
    }

    [Fact]
    public void WindowP50_IsMedianOfWindow()
    {
        var profiler = new Profiler(_bus);

        foreach (var ms in new[] { 1, 2, 3, 4, 5 })
            _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(ms)));

        Assert.Equal(TimeSpan.FromMilliseconds(3), profiler.Profiles["A"].WindowP50);
    }

    [Fact]
    public void WindowP95_ExceedsP50ForSkewedDistribution()
    {
        var profiler = new Profiler(_bus);

        // 9 fast ticks + 1 spike = 10 items; ceil(0.95 × 10) - 1 = 9 → spike.
        for (var i = 0; i < 9; i++)
            _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(1)));
        _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(100)));

        Assert.True(profiler.Profiles["A"].WindowP95 > profiler.Profiles["A"].WindowP50);
    }

    [Fact]
    public void WindowP99_IsAtLeastP95()
    {
        var profiler = new Profiler(_bus);

        for (var i = 1; i <= 100; i++)
            _bus.Publish(Evt("A", TimeSpan.FromMilliseconds(i)));

        var profile = profiler.Profiles["A"];
        Assert.True(profile.WindowP99 >= profile.WindowP95);
    }
}
