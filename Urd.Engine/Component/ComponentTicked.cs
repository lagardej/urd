namespace Urd.Engine.Component;

/// <summary>Published by <see cref="BaseComponent" /> after a tick completes.</summary>
public record ComponentTicked(string ComponentId, ulong ElapsedSeconds, TimeSpan Duration);