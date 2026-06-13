namespace Urd.Engine.Topology;

/// <summary>
///     Grid resolution levels for the spherical partition.
///     <para>
///         R0 is the coarsest level. Each resolution subdivides every cell into approximately 7 children,
///         following the formula <c>2 + 10 × 7^n</c> cells at resolution <c>n</c>.
///     </para>
///     <para>
///         The subdivision is not perfectly uniform: every resolution contains exactly 12 pentagonal cells,
///         a topological invariant of any convex spherical partition (consequence of Euler's polyhedron formula).
///         All remaining cells are hexagonal.
///     </para>
///     <para>
///         <b>Warning:</b> querying all cells at high resolutions is extremely expensive.
///         Prefer localised queries (disk, ring, children) over full-resolution enumeration above R5.
///     </para>
/// </summary>
public enum Resolution
{
    /// <summary>122 cells.</summary>
    R0 = 0,

    /// <summary>842 cells.</summary>
    R1 = 1,

    /// <summary>~5.9K cells.</summary>
    R2 = 2,

    /// <summary>~41K cells.</summary>
    R3 = 3,

    /// <summary>~288K cells.</summary>
    R4 = 4,

    /// <summary>~2M cells.</summary>
    R5 = 5,

    /// <summary>~14M cells.</summary>
    R6 = 6,

    /// <summary>~99M cells.</summary>
    R7 = 7,

    /// <summary>~692M cells.</summary>
    R8 = 8,

    /// <summary>~4.8B cells.</summary>
    R9 = 9,

    /// <summary>~34B cells. Avoid full-resolution queries.</summary>
    R10 = 10,

    /// <summary>~237B cells. Avoid full-resolution queries.</summary>
    R11 = 11,

    /// <summary>~1.7T cells. Avoid full-resolution queries.</summary>
    R12 = 12,

    /// <summary>~11.6T cells. Avoid full-resolution queries.</summary>
    R13 = 13,

    /// <summary>~81T cells. Avoid full-resolution queries.</summary>
    R14 = 14,

    /// <summary>~570T cells. Avoid full-resolution queries.</summary>
    R15 = 15
}
