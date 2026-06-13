using Urd.Engine.Clock;
using Urd.Engine.Messaging;

namespace Urd.Engine;

/// <summary>
/// Engine entry point. Call <see cref="Tick"/> at the desired rate to drive the simulation.
/// One call per second = real-time; N calls per second = N× time compression.
/// </summary>
public sealed class Urd
{
    private readonly IMessageBus _bus;
    private readonly Clock.Clock _clock = new();
    private readonly ulong _tickDuration;

    public Urd(IMessageBus bus, ulong tickDuration = 1)
    {
        _bus = bus;
        _tickDuration = tickDuration;
    }

    public void Tick()
    {
        _clock.Advance(_tickDuration);
        _bus.Publish(new ClockTicked(_clock.ElapsedSeconds));
    }
}