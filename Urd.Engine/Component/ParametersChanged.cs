namespace Urd.Engine.Component;

/// <summary>Published when a component's parameters change. Emitted by config loader at startup or by UI at runtime.</summary>
public record ParametersChanged<TParameters>(TParameters Parameters);