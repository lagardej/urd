namespace Urd.Engine.Component;

/// <summary>Payload passed to a component's tick.</summary>
public record TickContext(ulong ElapsedSeconds, CellSet CellSet);