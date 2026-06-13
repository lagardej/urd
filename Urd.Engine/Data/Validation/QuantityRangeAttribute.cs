using System.ComponentModel.DataAnnotations;
using UnitsNet;

namespace Urd.Engine.Data.Validation;

/// <summary>
///     Validates that an <see cref="IQuantity" /> property falls within a specified range.
///     The value is extracted in the given unit before comparison.
/// </summary>
/// <example>
///     <code>
///         [QuantityRange(0, 180, AngleUnit.Degree)]
///         public required Angle Obliquity { get; init; }
///
///         [QuantityRange(0, 1, RatioUnit.DecimalFraction, RangeBounds.ExclusiveMax)]
///         public required Ratio OrbitalEccentricity { get; init; }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class QuantityRangeAttribute : ValidationAttribute
{
    private readonly RangeBounds _bounds;
    private readonly double _max;
    private readonly double _min;
    private readonly Enum _unit;

    public QuantityRangeAttribute(double min, double max, object unit, RangeBounds bounds = RangeBounds.Inclusive)
    {
        _min = min;
        _max = max;
        _unit = (Enum)unit;
        _bounds = bounds;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is not IQuantity q)
            return new ValidationResult($"{ctx.MemberName} requires IQuantity.");

        var v = q.As(_unit);

        var minOk = _bounds is RangeBounds.ExclusiveMin or RangeBounds.Exclusive ? v > _min : v >= _min;
        var maxOk = _bounds is RangeBounds.ExclusiveMax or RangeBounds.Exclusive ? v < _max : v <= _max;

        if (minOk && maxOk)
            return ValidationResult.Success;

        var minBracket = _bounds is RangeBounds.ExclusiveMin or RangeBounds.Exclusive ? "(" : "[";
        var maxBracket = _bounds is RangeBounds.ExclusiveMax or RangeBounds.Exclusive ? ")" : "]";
        return new ValidationResult(
            $"{ctx.MemberName} must be in {minBracket}{_min}, {_max}{maxBracket} {_unit}.");
    }
}