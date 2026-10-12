namespace UnitConversionApi.Models;

public sealed record ConversionResponse(
    string Category,
    double InputValue,
    string FromUnit,
    double Result,
    string ToUnit);
