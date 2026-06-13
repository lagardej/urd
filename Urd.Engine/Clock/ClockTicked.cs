namespace Urd.Engine.Clock;

/// <summary>Published by <see cref="Urd"/> after the clock has advanced. Carries the current simulated time since epoch.</summary>
public record ClockTicked(ulong ElapsedSeconds);