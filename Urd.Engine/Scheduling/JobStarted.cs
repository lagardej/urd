namespace Urd.Engine.Scheduling;

/// <summary>Published by <see cref="Scheduler" /> immediately before a job is dispatched.</summary>
public record JobStarted(string ComponentId, ulong ElapsedSeconds);
