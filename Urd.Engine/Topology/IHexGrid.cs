namespace Urd.Engine.Topology;

/// <summary>
///     Spherical partition operation contract.
///     All coordinates are latitude/longitude in degrees.
///     Cell identifiers are opaque to callers.
///     All operations are deterministic and side-effect free.
/// </summary>
public interface IHexGrid
{
    // Partition access

    /// <summary>Returns the complete top-level partition (R0 cells).</summary>
    long[] RootCells();

    /// <summary>Resolves the cell at the given coordinates and resolution.</summary>
    ulong CellAt(double latDeg, double lngDeg, Resolution resolution);

    /// <summary>Enumerates all cells at a given resolution.</summary>
    IEnumerable<ulong> CellsAtResolution(Resolution resolution);

    // Cell introspection

    /// <summary>Returns the resolution of a given cell.</summary>
    Resolution ResolutionOf(ulong cell);

    /// <summary>Returns the centre coordinate of a cell.</summary>
    LatLng CenterOf(ulong cell);

    /// <summary>Returns the boundary polygon vertices of a cell in traversal order.</summary>
    LatLng[] BoundaryOf(ulong cell);

    /// <summary>Returns whether a cell identifier is valid.</summary>
    bool IsValid(ulong cell);

    // Hierarchy traversal

    /// <summary>Returns the ancestor cell at a given coarser resolution.</summary>
    ulong ParentOf(ulong cell, Resolution parentResolution);

    /// <summary>Returns all child cells at a given finer resolution.</summary>
    long[] ChildrenOf(ulong cell, Resolution childResolution);

    /// <summary>
    ///     Infers the grid resolution from the first cell in a non-empty dictionary.
    ///     Returns <paramref name="fallback"/> when the dictionary is empty.
    /// </summary>
    Resolution InferResolution<T>(Dictionary<ulong, T> cells, Resolution fallback);

    // Neighbourhood traversal

    /// <summary>Returns all cells within ring radius k of a centre cell, including the centre.</summary>
    long[] Disk(ulong cell, int k);

    /// <summary>
    ///     Enumerates cells at exactly grid-ring distance k from a centre cell.
    ///     k = 0 yields only the centre cell itself.
    /// </summary>
    IEnumerable<ulong> GridRing(ulong center, int k);
}
