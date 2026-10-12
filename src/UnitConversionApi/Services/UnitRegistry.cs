using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

/// <summary>
/// Central catalogue of supported units. Add units here without changing the API layer.
/// FactorToBase is used for linear conversions; temperature is handled separately.
/// </summary>
public sealed class UnitRegistry : IUnitRegistry
{
    private readonly Dictionary<string, Dictionary<string, UnitDefinition>> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["length"] = Build(
                ("m", "Meter", 1d), ("km", "Kilometer", 1000d),
                ("cm", "Centimeter", 0.01d), ("mm", "Millimeter", 0.001d),
                ("mi", "Mile", 1609.344d), ("yd", "Yard", 0.9144d),
                ("ft", "Foot", 0.3048d), ("in", "Inch", 0.0254d)),
            ["weight"] = Build(
                ("kg", "Kilogram", 1d), ("g", "Gram", 0.001d),
                ("mg", "Milligram", 0.000001d), ("lb", "Pound", 0.45359237d),
                ("oz", "Ounce", 0.028349523125d), ("t", "Metric tonne", 1000d)),
            ["mass"] = Build(
                ("kg", "Kilogram", 1d), ("g", "Gram", 0.001d),
                ("mg", "Milligram", 0.000001d), ("lb", "Pound", 0.45359237d),
                ("oz", "Ounce", 0.028349523125d), ("t", "Metric tonne", 1000d)),
            ["temperature"] = Build(
                ("c", "Celsius", 1d), ("f", "Fahrenheit", 1d), ("k", "Kelvin", 1d))
        };

    public IReadOnlyCollection<string> Categories => _units.Keys.ToArray();

    public IReadOnlyCollection<UnitDefinition> GetUnits(string category)
    {
        if (!_units.TryGetValue(category, out var units))
            throw new KeyNotFoundException($"Unknown category '{category}'.");
        return units.Values.OrderBy(x => x.Symbol).ToArray();
    }

    public bool TryGetUnit(string category, string unit, out UnitDefinition definition)
    {
        definition = default!;
        return _units.TryGetValue(category, out var units) &&
               units.TryGetValue(unit, out definition!);
    }

    private static Dictionary<string, UnitDefinition> Build(
        params (string Symbol, string Name, double Factor)[] definitions) =>
        definitions.ToDictionary(
            x => x.Symbol,
            x => new UnitDefinition(x.Symbol, x.Name, x.Factor),
            StringComparer.OrdinalIgnoreCase);
}
