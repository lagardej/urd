namespace Urd.Engine.Component;

/// <summary>The set of cells a component operates on for a given tick.</summary>
public record CellSet(IReadOnlySet<int> Cells)
{
    public static CellSet Global { get; } = new(new HashSet<int>());
}