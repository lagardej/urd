using System.Diagnostics;
using Urd.Engine.Clock;
using Urd.Engine.Messaging;

namespace Urd.Engine.Inspection;

/// <summary>
///     Tracks simulated time and derives wall-clock compression ratio.
///     Compression = simulated seconds elapsed / wall-clock seconds elapsed.
/// </summary>
public sealed class ClockInspector
{
    private readonly Func<TimeSpan> _wallClock;

    public ClockInspector(IMessageBus bus)
        : this(bus, null) { }

    public ClockInspector(IMessageBus bus, Func<TimeSpan>? wallClock)
    {
        var sw = Stopwatch.StartNew();
        _wallClock = wallClock ?? (() => sw.Elapsed);
        bus.Subscribe<ClockTicked>(evt => ElapsedSeconds = evt.ElapsedSeconds);
    }

    public ulong    ElapsedSeconds   { get; private set; }
    public TimeSpan WallClockElapsed => _wallClock();

    /// <summary>Simulated seconds per wall-clock second. Zero until the first tick.</summary>
    public double CompressionRatio =>
        _wallClock().TotalSeconds > 0
            ? ElapsedSeconds / _wallClock().TotalSeconds
            : 0;
}
