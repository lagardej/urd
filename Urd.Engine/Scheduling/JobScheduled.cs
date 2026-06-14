using Urd.Engine.Component;

namespace Urd.Engine.Scheduling;

/// <summary>Published by a component to register a one-shot job with the <see cref="Scheduler" />.</summary>
public record JobScheduled(ulong DueAt, Action<TickContext> Tick, CellSet CellSet);