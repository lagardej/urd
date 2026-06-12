using System.ComponentModel.DataAnnotations;
using Urd.Engine.Data.Normalization;

namespace Urd.Engine.Data.Validation;

/// <summary>
///     Validates constraints declared via attributes on a record's properties.
///     Throws <see cref="ValidationException" /> on the first violation.
///     Run <see cref="Normalizer.Normalize{T}" /> before validating.
/// </summary>
public static class Validator
{
    public static void Validate<T>(T record) where T : notnull
    {
        System.ComponentModel.DataAnnotations.Validator.ValidateObject(
            record, new ValidationContext(record), true);
    }
}