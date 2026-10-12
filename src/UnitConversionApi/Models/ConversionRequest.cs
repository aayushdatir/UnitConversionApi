using System.ComponentModel.DataAnnotations;

namespace UnitConversionApi.Models;

public sealed class ConversionRequest
{
    [Required, StringLength(30, MinimumLength = 1)]
    public string Category { get; init; } = string.Empty;

    public double Value { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public string FromUnit { get; init; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 1)]
    public string ToUnit { get; init; } = string.Empty;
}
