using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public sealed class ConversionService(IUnitRegistry registry) : IConversionService
{
    public ConversionResponse Convert(ConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!double.IsFinite(request.Value))
            throw new ArgumentException("Value must be a finite number.");

        var category = request.Category.Trim().ToLowerInvariant();
        var from = request.FromUnit.Trim();
        var to = request.ToUnit.Trim();

        if (!registry.TryGetUnit(category, from, out _))
            throw new KeyNotFoundException($"Unit '{from}' is not supported for category '{category}'.");
        if (!registry.TryGetUnit(category, to, out _))
            throw new KeyNotFoundException($"Unit '{to}' is not supported for category '{category}'.");

        var result = category == "temperature"
            ? ConvertTemperature(request.Value, from, to)
            : ConvertLinear(request.Value, registry, category, from, to);

        if (!double.IsFinite(result))
            throw new ArgumentException("The conversion produced a value outside the supported numeric range.");

        return new ConversionResponse(category, request.Value, from, Math.Round(result, 8), to);
    }

    private static double ConvertLinear(
        double value, IUnitRegistry registry, string category, string from, string to)
    {
        registry.TryGetUnit(category, from, out var source);
        registry.TryGetUnit(category, to, out var target);
        return value * source.FactorToBase / target.FactorToBase;
    }

    private static double ConvertTemperature(double value, string from, string to)
    {
        // Convert to Celsius first, then to the requested unit.
        var celsius = from.ToLowerInvariant() switch
        {
            "c" => value,
            "f" => (value - 32d) * 5d / 9d,
            "k" => value - 273.15d,
            _ => throw new KeyNotFoundException($"Unsupported temperature unit '{from}'.")
        };

        if (celsius < -273.15d)
            throw new ArgumentException("Temperature cannot be below absolute zero (-273.15 °C).");

        return to.ToLowerInvariant() switch
        {
            "c" => celsius,
            "f" => celsius * 9d / 5d + 32d,
            "k" => celsius + 273.15d,
            _ => throw new KeyNotFoundException($"Unsupported temperature unit '{to}'.")
        };
    }
}
