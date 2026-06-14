namespace Urd.Engine.Scheduling;

/// <summary>Published by a component to register a one-shot job with the <see cref="Urd" />.</summary>
public record JobScheduled(ulong DueAt, Action<ulong> Tick);